﻿#include <iostream>
#include "bs.h"

using namespace std;

BS::BS(int size_, double r_, PnlVect* dividend_, PnlVect* spot_)
{
	this->size_ = size_;
	this->r_ = r_;
	this->spot_ = pnl_vect_copy(spot_);
	this->dividend_ = dividend_;
	if (this->size_ == 1)
		assert(this->rho_ == 1);

	sigma_ = pnl_vect_create(this->size_);
	L = pnl_mat_create(this->size_, this->size_);


}
void BS::calibrate(const PnlMat* past, double dt)// call it after construction of bs
{
	PnlMat* Cov = pnl_mat_create_from_zero(size_, size_);
	PnlMat* lSk1k = pnl_mat_create_from_zero(past->m - 1, size_);
	double sum;
	double temp;
	for (int d = 0; d < size_; d++)
	{
		sum = 0;
		for (int k = 0; k < past->m - 1; k++)
		{
			temp = log(MGET(past, k + 1, d) / MGET(past, k, d));
			MLET(lSk1k, k, d) += temp;
			sum += temp;
		}
		for (int k = 0; k < past->m - 1; k++)
		{
			MLET(lSk1k, k, d) += sum / (past->m);
		}
	}
	for (int i = 0; i < size_; i++)
	{
		for (int j = i; j < size_; j++)
		{
			for (int k = 0; k < past->m - 1; k++)
			{
				MLET(Cov, i, j) += MGET(lSk1k, k, i)*MGET(lSk1k, k, j);
			}
			MLET(Cov, i, j) /= dt*(past->m - 1);
			if (i != j){
				MLET(Cov, j, i) = MGET(Cov, i, j);
			}
		}
	}
	for (int d = 0; d < size_; d++)
	{
		LET(sigma_, d) = sqrt(MGET(Cov, d, d));
	}
	//pnl_mat_print(Cov);
	cout << "Cov " << endl;
	pnl_mat_set_zero(L);
	for (int i = 0; i < size_; i++)
	{
		for (int j = i; j < size_; j++)
		{
			MLET(L, i, j) = MGET(Cov, i, j) / (GET(sigma_, i)*GET(sigma_, j));
			MLET(L, j, i) = MGET(L, i, j);
		}
	}
	pnl_mat_chol(L);
	// pnl_mat_print(Cov);
	// pnl_mat_set_id(L);

	pnl_mat_free(&Cov);
	pnl_mat_free(&lSk1k);
}

BS::~BS()
{
#ifdef _DEBUG
	cout << "~BS() : Ready to call pnl_vect_free on sigma, spot and trend  " << endl;
#endif
	pnl_vect_free(&this->spot_);
	pnl_vect_free(&this->sigma_);
	pnl_mat_free(&L);
#ifdef _DEBUG
	cout << "~BS() : Successfull call of pnl_rng_free" << endl;
#endif
}


void BS::asset(PnlMat *path, double T, int N, PnlRng *rng)
{
	double step = T / N;
	double prodScal = 0;
	double sigma_d = 0;
	double dividend_d = 0;

	// The Gaussian vector
	PnlVect *G = pnl_vect_create_from_zero(this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);
	// First col of path contains spot
	for (int d = 0; d < this->size_; d++)
		MLET(path, 0, d) = GET(this->spot_, d);

	// Generation from 2 to N+1(th) column
	for (int ti = 1; ti < N + 1; ti++)
	{
		// Gaussian Dimension size_ generation
		pnl_vect_rng_normal(G, this->size_, rng);
		for (int d = 0; d < this->size_; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			MLET(path, ti, d) = MGET(path, ti - 1, d)*exp((r_ - dividend_d - pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}
	}

	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&Ld);
}

void BS::asset(PnlMat *path, double t, int N, double T, PnlRng *rng, const PnlMat *past)
{
	if (t == 0.0)
	{
		this->asset(path, T, N, rng);
		return;
	}
	double step = T / N;
	double dt = t / step;
	double Error = abs(dt - round(dt)) / (dt);
	bool copySt = false;
	int lastIndexOfPast = 0;
	if (Error <= 0.05)
	{
		lastIndexOfPast = round(dt);
		copySt = true;
	}
	else{
		lastIndexOfPast = floor(dt);
		copySt = false;
	}
	double prodScal = 0;
	double sigma_d = 0;
	double dividend_d = 0;
	// Copy of the past on the generated path
	for (int ti = 0; ti <= lastIndexOfPast; ti++)
	{
		for (int d = 0; d < this->size_; d++)
		{
			MLET(path, ti, d) = MGET(past, ti, d);
		}
	}
	// The vector St
	PnlVect *St = pnl_vect_create_from_zero(this->size_);
	if (!copySt)
	{
		for (int d = 0; d < this->size_; d++)
			pnl_vect_set(St, d, MGET(past, lastIndexOfPast + 1, d));
		//pnl_vect_set(St,d,MGET(past,past->m-1,d));
	}
	// The Gaussian vector
	PnlVect *G = pnl_vect_create_from_zero(this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);
	// Generation from 2 to N+1(th) column
	for (int ti = lastIndexOfPast + 1; ti < N + 1; ti++)
	{
		// Gaussian Dimension size_ generation
		pnl_vect_rng_normal(G, this->size_, rng);
		for (int d = 0; d < this->size_; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			if ((ti == lastIndexOfPast + 1) && (!copySt))
				MLET(path, ti, d) = GET(St, d)*exp((this->r_ - dividend_d - pow(sigma_d, 2) / 2)*(ti*step - t) + sigma_d*sqrt(ti*step - t)*prodScal);
			else
				MLET(path, ti, d) = MGET(path, ti - 1, d)*exp((r_ - dividend_d - pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}
	}
	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&Ld);
	pnl_vect_free(&St);
}

void BS::shift_asset(PnlMat *shift_path, const PnlMat *path, int d, double h, double t, double T, double timestep)
{
	int pastSize = floor((timestep / T)*t);
	for (int ti = 0; ti < timestep + 1; ti++)
	{
		for (int j = 0; j < this->size_; j++)
		{
			if ((ti > pastSize) && (j == d))
				MLET(shift_path, ti, j) = (1 + h)*MGET(path, ti, j);
			else
				MLET(shift_path, ti, j) = MGET(path, ti, j);
		}
	}
}
void BS::forward(PnlMat *path, double T, int N, PnlRng *rng) // generation des trajectoires des actifs risqués sous la mesure forward
{
	double step = T / N;
	double prodScal = 0;
	double sigma_d = 0;
	double dividend_d = 0;
	int nbAssets = this->size_ - this->stochIRates_.size() + 1;
	double vol = this->stochIRates_[0]->vol_;
	double a = this->stochIRates_[0]->reversionSpeed_;
	double vol_d;
	double sigmaFwd;
	double intA, intA2, intAAd, intAd, intAd2;
	double zeroCoupon0_T = 0;
	double zeroCoupon1_T = 0;
	double zeroCouponF0_T = 0;
	double zeroCouponF1_T = 0;
	this->stochIRates_[0]->priceZeroCoupon(zeroCoupon1_T, 0, T);
	// The Gaussian vector
	PnlVect *GAll = pnl_vect_create_from_zero(2 * this->size_ - nbAssets + 1);
	PnlVect *G = pnl_vect_create_from_zero(2 * this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);


	// Generation from 2 to N+1(th) column
	for (int ti = 1; ti < N + 1; ti++)
	{
		zeroCoupon0_T = zeroCoupon1_T;
		this->stochIRates_[0]->priceZeroCoupon(zeroCoupon1_T, ti*step, T);
		intA = this->stochIRates_[0]->getIntA((ti - 1)*step, ti*step);
		intA2 = this->stochIRates_[0]->getIntA2((ti - 1)*step, ti*step);
		// Gaussian Dimension size_ generation
		// dans un premier temps on considere les zero coupon sont independent des actifs
		pnl_vect_rng_normal(GAll, 2*this->size_- nbAssets + 1, rng);
		pnl_vect_extract_subvect(G, GAll, 0, this->size_);

		for (int d = 0; d < nbAssets; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			sigmaFwd = sigma_d*sigma_d*step + vol*vol*intA2;
			MLET(path, ti, d) = (zeroCoupon1_T * MGET(path, ti - 1, d) / zeroCoupon0_T) 
				* exp(-dividend_d *step - sigmaFwd / 2 + sigma_d*sqrt(step)*prodScal + vol*sqrt(intA2)*GET(GAll, this->size_));
		}
		for (int d = nbAssets; d < this->size_; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			vol_d = this->stochIRates_[d - nbAssets + 1]->vol_;
			this->stochIRates_[0]->priceZeroCoupon(zeroCouponF0_T, (ti - 1)*step, T);
			this->stochIRates_[0]->priceZeroCoupon(zeroCouponF1_T, ti*step, T);
			intAd = this->stochIRates_[d - nbAssets + 1]->getIntA((ti - 1)*step, ti*step);
			intAd2 = this->stochIRates_[d - nbAssets + 1]->getIntA2((ti - 1)*step, ti*step);
			intAAd = this->stochIRates_[0]->getIntAAf(this->stochIRates_[d - nbAssets + 1], (ti - 1)*step, ti*step);
			sigmaFwd = sigma_d*sigma_d*step + vol_d*vol_d*intAd2 + vol*vol*intA2;
			MLET(path, ti, d) = (zeroCoupon1_T *zeroCouponF0_T*MGET(path, ti - 1, d) / (zeroCoupon0_T*zeroCouponF1_T)) 
				* exp(-sigmaFwd / 2 + sigma_d*sqrt(step)*prodScal 
				+ vol*sqrt(intA2)*GET(GAll, this->size_) + vol_d*sqrt(intAd2)*GET(GAll, this->size_ + d - nbAssets + 1));
		}
	}
	// à l'instant N les zero coupon vont valoir 1

	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&Ld);
}

void BS::forward(PnlMat *path, double t, int N, double T, PnlRng *rng, const PnlMat *past)// generation des trajectoires des actifs risqués sous la mesure forward
{
	int nbAssets = this->size_ - this->stochIRates_.size() + 1;
	double vol = this->stochIRates_[0]->vol_;
	double a = this->stochIRates_[0]->reversionSpeed_;
	double vol_d;
	double sigmaFwd;
	double a_d, intA, intA2, intAAd, intAd, intAd2;
	double zeroCoupon0_T = 0;
	double zeroCoupon1_T = 0;
	double zeroCouponF0_T = 0;
	double zeroCouponF1_T = 0;
	this->stochIRates_[0]->priceZeroCoupon(zeroCoupon1_T, 0, T);
	if (t == 0.0)
	{
		this->forward(path, T, N, rng);
		return;
	}
	double step = T / N;
	double dt = t / step;
	double Error = abs(dt - round(dt)) / (dt);
	bool copySt = false;
	int lastIndexOfPast = 0;
	double Sti;
	if (Error <= 0.05)
	{
		lastIndexOfPast = round(dt);
		copySt = true;
	}
	else{
		lastIndexOfPast = floor(dt);
		copySt = false;
	}
	double prodScal = 0;
	double sigma_d = 0;
	double dividend_d = 0;
	// Copy of the past on the generated path
	for (int ti = 0; ti <= lastIndexOfPast; ti++)
	{
		for (int d = 0; d < this->size_; d++)
		{
			MLET(path, ti, d) = MGET(past, ti, d);
		}
	}

	// The vector St
	PnlVect *St = pnl_vect_create_from_zero(this->size_);
	if (!copySt)
	{
		for (int d = 0; d < this->size_; d++)
			pnl_vect_set(St, d, MGET(path, lastIndexOfPast + 1, d));
	}
	// The Gaussian vector
	PnlVect *GAll = pnl_vect_create_from_zero(2 * this->size_ - nbAssets + 1);
	PnlVect *G = pnl_vect_create_from_zero(this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);
	// Generation from lastIndexOfPast + 1 to N+1(th) column
	for (int ti = lastIndexOfPast + 1; ti < N + 1; ti++)
	{
		zeroCoupon0_T = zeroCoupon1_T;
		this->stochIRates_[0]->priceZeroCoupon(zeroCoupon1_T, ti*step, T);
		intA = this->stochIRates_[0]->getIntA((ti - 1)*step, ti*step);
		intA2 = this->stochIRates_[0]->getIntA2((ti - 1)*step, ti*step);
		// Gaussian Dimension size_ generation
		// dans un premier temps on considere les zero coupon sont independent des actifs
		pnl_vect_rng_normal(GAll, 2 * this->size_ - nbAssets + 1, rng);
		pnl_vect_extract_subvect(G, GAll, 0, this->size_);
		for (int d = 0; d < nbAssets; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			dividend_d = GET(dividend_, d);
			//sigmaFwd = sigma_d*sigma_d*step + 2 * sigma_d * vol * intA + vol*vol*intA2;
			sigmaFwd = sigma_d*sigma_d*step + vol*vol*intA2;
			if ((ti == lastIndexOfPast + 1) && (!copySt))
				MLET(path, ti, d) = (zeroCoupon1_T * GET(St, d) / zeroCoupon0_T)
				* exp(-dividend_d *step - sigmaFwd / 2 + sigma_d*sqrt(step)*prodScal + vol*sqrt(intA2)*GET(GAll, this->size_));
			//MLET(path, ti, d) = (zeroCoupon1_T * GET(St, d) / zeroCoupon0_T)*exp(-dividend_d *step - sigmaFwd / 2 + sqrt(sigmaFwd)*prodScal*prodScal);
			else
				MLET(path, ti, d) = (zeroCoupon1_T * MGET(path, ti - 1, d) / zeroCoupon0_T)
				* exp(-dividend_d *step - sigmaFwd / 2 + sigma_d*sqrt(step)*prodScal + vol*sqrt(intA2)*GET(GAll, this->size_));
			//MLET(path, ti, d) = (zeroCoupon1_T * MGET(path, ti - 1, d) / zeroCoupon0_T)* exp(-dividend_d *step - sigmaFwd / 2 + sqrt(sigmaFwd)*prodScal);
		}
		for (int d = nbAssets; d < this->size_; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			vol_d = this->stochIRates_[d - nbAssets + 1]->vol_;
			this->stochIRates_[0]->priceZeroCoupon(zeroCouponF0_T, (ti - 1)*step, T);
			this->stochIRates_[0]->priceZeroCoupon(zeroCouponF1_T, ti*step, T);
			intAd = this->stochIRates_[d - nbAssets + 1]->getIntA((ti - 1)*step, ti*step);
			intAd2 = this->stochIRates_[d - nbAssets + 1]->getIntA2((ti - 1)*step, ti*step);
			intAAd = this->stochIRates_[0]->getIntAAf(this->stochIRates_[d - nbAssets + 1], (ti - 1)*step, ti*step);
			sigmaFwd = sigma_d*sigma_d*step + vol_d*vol_d*intAd2 + vol*vol*intA2;
			if ((ti == lastIndexOfPast + 1) && (!copySt))
				MLET(path, ti, d) = (zeroCoupon1_T *zeroCouponF0_T*GET(St, d) / (zeroCoupon0_T*zeroCouponF1_T))
				* exp(-sigmaFwd / 2 + sigma_d*sqrt(step)*prodScal
				+ vol*sqrt(intA2)*GET(GAll, this->size_) + vol_d*sqrt(intAd2)*GET(GAll, this->size_ + d - nbAssets + 1));
			else
				MLET(path, ti, d) = (zeroCoupon1_T *zeroCouponF0_T*MGET(path, ti - 1, d) / (zeroCoupon0_T*zeroCouponF1_T))
				* exp(-sigmaFwd / 2 + sigma_d*sqrt(step)*prodScal
				+ vol*sqrt(intA2)*GET(GAll, this->size_) + vol_d*sqrt(intAd2)*GET(GAll, this->size_ + d - nbAssets + 1));
		}
	}
	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&GAll);
	pnl_vect_free(&Ld);
}

void BS::assetStochRate(PnlMat *path, double T, int N, PnlRng *rng) {
	double step = T / N;
	double prodScal = 0;
	double sigma_d = 0;
	double dividend_d = 0;

	int nbAssets = this->size_ - this->stochIRates_.size() + 1;
	double discount = 0., fxDiscount;

	// The Gaussian vector
	PnlVect *G = pnl_vect_create_from_zero(this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);
	// First col of path contains spot
	for (int d = 0; d < this->size_; d++)
		MLET(path, 0, d) = GET(this->spot_, d);

	// Generation from 2 to N+1(th) column
	for (int ti = 1; ti < N + 1; ti++)
	{
		discount = this->stochIRates_[0]->getDiscountFactor((ti - 1)*step, N, ti*step, rng);
		discount = exp(discount);
		// Gaussian Dimension size_ generation
		pnl_vect_rng_normal(G, this->size_, rng);
		for (int d = 0; d < nbAssets; d++) {
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			MLET(path, ti, d) = MGET(path, ti - 1, d)* discount * exp((-dividend_d - pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}
		
		for (int d = nbAssets; d < this->size_; d++)
		{
			sigma_d = GET(sigma_, d);
			double tmp = (step / this->stochIRates_[d - nbAssets + 1]->reversionSpeed_) - 
				((1 - exp(-this->stochIRates_[d - nbAssets + 1]->reversionSpeed_ * step)) / pow(this->stochIRates_[d - nbAssets + 1]->reversionSpeed_, 2));
			fxDiscount = this->stochIRates_[d - nbAssets + 1]->getDiscountFactor((ti - 1)*step, N, ti*step, rng) + tmp * sigma_d * this->stochIRates_[d - nbAssets + 1]->vol_;
			//		(sigma_d * step * step * this->stochIRates_[d - nbAssets + 1]->vol_);
			fxDiscount = exp(-fxDiscount);
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			//MLET(path, ti, d) = MGET(path, ti - 1, d) * discount * fxDiscount * exp((-pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
			MLET(path, ti, d) = MGET(path, ti - 1, d) * discount * fxDiscount * exp((-pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}
	}

	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&Ld);
}

void BS::assetAnti(PnlMat *path, double T, int N, PnlRng *rng)
{
	double step = T / N;
	double prodScal = 0;
	double sigma_d = 0;
	double dividend_d = 0;

	// The Gaussian vector
	PnlVect *G = pnl_vect_create_from_zero(this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);

	pnl_mat_mult_scalar(normalMat_, -1);
	// First col of path contains spot
	for (int d = 0; d < this->size_; d++)
		MLET(path, 0, d) = GET(this->spot_, d);

	// Generation from 2 to N+1(th) column
	for (int ti = 1; ti < N + 1; ti++)
	{
		// Gaussian Dimension size_ generation
		//	pnl_vect_rng_normal(G, this->size_, rng);
		pnl_mat_get_row(G, normalMat_, ti - 1);
		for (int d = 0; d < this->size_; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			MLET(path, ti, d) = MGET(path, ti - 1, d)*exp((r_ - dividend_d - pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}
	}

	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&Ld);
}


void BS::assetStochRate(PnlMat *path, double t, double T, int N, PnlRng *rng, const PnlMat *past) {
	if (t == 0.0)
	{
		this->asset(path, T, N, rng);
		return;
	}
	double step = T / N;
	double dt = t / step;
	double Error = abs(dt - round(dt)) / (dt);
	bool copySt = false;
	int lastIndexOfPast = 0;
	if (Error <= 0.05)
	{
		lastIndexOfPast = round(dt);
		copySt = true;
	}
	else{
		lastIndexOfPast = floor(dt);
		copySt = false;
	}
	double prodScal = 0;
	double sigma_d = 0;
	double dividend_d = 0;
	// Copy of the past on the generated path
	for (int ti = 0; ti <= lastIndexOfPast; ti++)
	{
		for (int d = 0; d < this->size_; d++)
		{
			MLET(path, ti, d) = MGET(past, ti, d);
		}
	}
	// The vector St
	PnlVect *St = pnl_vect_create_from_zero(this->size_);
	if (!copySt)
	{
		for (int d = 0; d < this->size_; d++)
			pnl_vect_set(St, d, MGET(past, lastIndexOfPast + 1, d));
		//pnl_vect_set(St,d,MGET(past,past->m-1,d));
	}

	// The Gaussian vector
	PnlVect *G = pnl_vect_create_from_zero(this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);

	int nbAssets = this->size_ - this->stochIRates_.size() + 1;
	double discount = 0., fxDiscount;

	// Generation from 2 to N+1(th) column
	for (int ti = lastIndexOfPast + 1; ti < N + 1; ti++)
	{
		discount = this->stochIRates_[0]->getDiscountFactor((ti - 1)*step, N, ti*step, rng);
		discount = exp(discount);
		// Gaussian Dimension size_ generation
		pnl_vect_rng_normal(G, this->size_, rng);
		for (int d = 0; d < nbAssets; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(sigma_, d);
			dividend_d = GET(dividend_, d);
			if ((ti == lastIndexOfPast + 1) && (!copySt))
				MLET(path, ti, d) = GET(St, d) * discount * exp((-dividend_d - pow(sigma_d, 2) / 2)*(ti*step - t) + sigma_d*sqrt(ti*step - t)*prodScal);
			else
				MLET(path, ti, d) = MGET(path, ti - 1, d) * discount *exp((-dividend_d - pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}

		for (int d = nbAssets; d < this->size_; d++)
		{
			sigma_d = GET(sigma_, d);
			double tmp = (step / this->stochIRates_[d - nbAssets + 1]->reversionSpeed_) - ((1 - exp(-this->stochIRates_[d - nbAssets + 1]->reversionSpeed_ * step)) / pow(this->stochIRates_[d - nbAssets + 1]->reversionSpeed_, 2));
			fxDiscount = this->stochIRates_[d - nbAssets + 1]->getDiscountFactor((ti - 1)*step, N, ti*step, rng) + tmp * sigma_d * this->stochIRates_[d - nbAssets + 1]->vol_;
			//		(sigma_d * step * step * this->stochIRates_[d - nbAssets + 1]->vol_);
			fxDiscount = exp(-fxDiscount);
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			dividend_d = GET(dividend_, d);
			if ((ti == lastIndexOfPast + 1) && (!copySt))
				MLET(path, ti, d) = GET(St, d) * discount * fxDiscount * exp((-pow(sigma_d, 2) / 2)*(ti*step - t) + sigma_d*sqrt(ti*step - t)*prodScal);
			else
				MLET(path, ti, d) = MGET(path, ti - 1, d) * discount * fxDiscount * exp((-pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}
	}
	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&Ld);
	pnl_vect_free(&St);
}



void BS::simul_market(PnlMat *path, double T, int H, PnlRng *rng)
{
	double step = T / H;
	double prodScal = 0;
	double sigma_d = 0;
	double mu_d = 0;
	double dividend_d = 0;

	// The Gaussian vector
	PnlVect *G = pnl_vect_create_from_zero(this->size_);
	PnlVect *Ld = pnl_vect_create_from_zero(this->size_);
	// First col of path contains spot
	for (int d = 0; d < this->size_; d++)
		MLET(path, 0, d) = GET(this->spot_, d);

	// Generation from 2 to H+1(th) column
	for (int ti = 1; ti < H + 1; ti++)
	{
		// Gaussian Dimension size_ generation
		pnl_vect_rng_normal(G, this->size_, rng);
		for (int d = 0; d < this->size_; d++)
		{
			pnl_mat_get_row(Ld, L, d);
			prodScal = pnl_vect_scalar_prod(Ld, G);
			sigma_d = GET(this->sigma_, d);
			mu_d = GET(this->trend_, d);
			dividend_d = GET(dividend_, d);
			MLET(path, ti, d) = MGET(path, ti - 1, d)*exp((mu_d - dividend_d - pow(sigma_d, 2) / 2)*step + sigma_d*sqrt(step)*prodScal);
		}
	}
	// Memory free
	pnl_vect_free(&G);
	pnl_vect_free(&Ld);
}
