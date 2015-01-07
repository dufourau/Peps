#include <iostream>
#include "mc.h"

using namespace std;

MonteCarlo::MonteCarlo(double T_, int TimeSteps_, int size_, int optionType_, double r_, double rho_, double* sigma_, double* spot_, double* trend_, int samples_)
{
	PnlVect* sigma= pnl_vect_create_from_ptr(size_,sigma_);
	PnlVect* spot= pnl_vect_create_from_ptr(size_, spot_);
	PnlVect* trend= pnl_vect_create_from_ptr(size_, trend_);
	// Construction de BS
	this->mod_ = new BS(size_, r_, rho_, sigma, spot, trend);
	this->opt_ = new Moduleis(T_, TimeSteps_,size_, optionType_);
	// Initialisation du generateur a MERSENNE : type 7 page 63
	rng = pnl_rng_create(PNL_RNG_MERSENNE);
	pnl_rng_sseed(rng, time(NULL));
	// Sample number et pas de difference finie
	this->samples_ = samples_;
	this->h_ = 0.1;
}

MonteCarlo::~MonteCarlo()
{
#ifdef _DEBUG
	cout << "~MonteCarlo() : Ready to call pnl_rng_free " << endl;
#endif
	pnl_rng_free(&rng);
	delete this->mod_;
	delete this->opt_;
#ifdef _DEBUG
	cout << "~MonteCarlo() : Successfull call of pnl_rng_free" << endl;
#endif
}

void MonteCarlo::price(double &prix, double &ic)
{
	PnlMat *generatedPath = pnl_mat_create(this->opt_->TimeSteps_ + 1, this->opt_->size_);
	double payoff = 0;
	double sumPayoff = 0;
	double sumPayoffSquare = 0;
	prix = 0;
	ic = 0;
	for (int i = 0; i < this->samples_; i++)
	{
		this->mod_->asset(generatedPath, this->opt_->T_, this->opt_->TimeSteps_, this->rng);
		payoff = this->opt_->payoff(generatedPath);
		sumPayoff += payoff;
		sumPayoffSquare += payoff*payoff;
	}
	prix = exp(-this->mod_->r_*this->opt_->T_)*(sumPayoff / this->samples_);
	double x = exp(-2 * this->mod_->r_*this->opt_->T_)*((sumPayoffSquare / this->samples_) - (sumPayoff / this->samples_)*(sumPayoff / this->samples_));
	ic = 2 * 1.96*sqrt(x) / sqrt(this->samples_);
	pnl_mat_free(&generatedPath);
}

void MonteCarlo::price(const PnlMat *past, double t, double &prix, double &ic)
{
	PnlMat *generatedPath = pnl_mat_create(this->opt_->TimeSteps_ + 1, this->opt_->size_);
	double payoff = 0;
	double sumPayoff = 0;
	double sumPayoffSquare = 0;
	double precision = this->samples_*(1.0e-16);
	prix = 0;
	ic = 0;
	for (int i = 0; i < this->samples_; i++)
	{
		this->mod_->asset(generatedPath, t, this->opt_->TimeSteps_, this->opt_->T_, this->rng, past);
		//pnl_mat_print(generatedPath);
		payoff = this->opt_->payoff(generatedPath);
		sumPayoff += payoff;
		sumPayoffSquare += payoff*payoff;
	}
	prix = exp(-this->mod_->r_*(this->opt_->T_ - t))*(sumPayoff / this->samples_);
	double x = exp(-2 * this->mod_->r_*(this->opt_->T_ - t))*(sumPayoffSquare / this->samples_) - prix*prix;
	if (abs(x) < precision)
		x = 0;
	ic = 2 * 1.96*sqrt(x) / sqrt(this->samples_);
	pnl_mat_free(&generatedPath);
}

void MonteCarlo::delta_(const PnlMat *past, double t, PnlVect *delta)
{
	pnl_vect_set_all(delta, 0);
	double T = opt_->T_;
	int size = mod_->size_;
	int N = opt_->TimeSteps_;

	PnlMat *path = pnl_mat_create(N + 1, size);

	for (int i = 0; i < samples_; i++)
	{
		mod_->asset(path, t, N, T, rng, past);
		for (int d = 0; d < size; d++)
		{
			for (int k = int(N*t / T); k < N + 1; k++)
			{
				pnl_mat_set(path, k, d, (1 + h_) * pnl_mat_get(path, k, d));
			}

			pnl_vect_set(delta, d, opt_->payoff(path) + pnl_vect_get(delta, d));

			for (int k = int(N*t / T); k < N + 1; k++)
			{
				pnl_mat_set(path, k, d, (1 - h_) / (1 + h_) * pnl_mat_get(path, k, d));
			}

			pnl_vect_set(delta, d, pnl_vect_get(delta, d) - opt_->payoff(path));

			for (int k = int(N*t / T); k < N + 1; k++)
			{
				pnl_mat_set(path, k, d, pnl_mat_get(path, k, d) / (1 - h_));
			}
		}

	}

	for (int d = 0; d < size; d++)
	{
		pnl_vect_set(delta, d, exp((-mod_->r_)*(opt_->T_ - t)) / (samples_ * 2 * h_ * pnl_mat_get(past, int(N*t / T), d)));

	}

	pnl_mat_free(&path);
}

void MonteCarlo::delta(const PnlMat *past, double t, PnlVect *delta)
{
	pnl_vect_set_all(delta, 0);
	PnlMat *generatedPath = pnl_mat_create(this->opt_->TimeSteps_ + 1, this->opt_->size_);
	PnlMat *shiftPath_Right = pnl_mat_create(this->opt_->TimeSteps_ + 1, this->opt_->size_);
	PnlMat *shiftPath_Left = pnl_mat_create(this->opt_->TimeSteps_ + 1, this->opt_->size_);
	double tmp = 0;
	// The vector St
	PnlVect *St = pnl_vect_create_from_zero(this->opt_->size_);
	int lastIndexOfPast = 0;
	if (t == 0.0)
		pnl_vect_clone(St, this->mod_->spot_);
	else
	{
		double step = this->opt_->T_ / this->opt_->TimeSteps_;
		double dt = t / step;
		double Error = abs(dt - round(dt)) / (dt);
		if (Error <= 0.05)
			lastIndexOfPast = round(dt);
		else
			lastIndexOfPast = floor(dt) + 1;
	}

	for (int j = 0; j<this->samples_; j++)
	{
		//Generation d'une trajectoire
		this->mod_->asset(generatedPath, t, this->opt_->TimeSteps_, this->opt_->T_, this->rng, past);
		for (int d = 0; d<this->mod_->size_; d++)
		{
			// Shift à droite
			this->mod_->shift_asset(shiftPath_Right, generatedPath, d, this->h_, t, this->opt_->T_, this->opt_->TimeSteps_);
			// Shift à droite
			this->mod_->shift_asset(shiftPath_Left, generatedPath, d, -this->h_, t, this->opt_->T_, this->opt_->TimeSteps_);
			// Delta payoff Right Left
			tmp = this->opt_->payoff(shiftPath_Right) - this->opt_->payoff(shiftPath_Left);
			pnl_vect_set(delta, d, GET(delta, d) + tmp);
		}
	}
	for (int d = 0; d<this->opt_->size_; d++)
	{
		pnl_vect_set(St, d, MGET(past, lastIndexOfPast, d));
		pnl_vect_set(delta, d, GET(delta, d)*exp(-this->mod_->r_*(this->opt_->T_ - t)) / (this->samples_*this->h_ * 2 * GET(St, d)));
	}
	pnl_mat_free(&generatedPath);
	pnl_mat_free(&shiftPath_Right);
	pnl_mat_free(&shiftPath_Left);
	pnl_vect_free(&St);
}

void MonteCarlo::hedge(PnlVect *V, double &PL, int H, const PnlMat *marketPath)
{
	PnlVect *delta_cour = pnl_vect_create_from_zero(this->opt_->size_);
	PnlVect *delta_prec = pnl_vect_create_from_zero(this->opt_->size_);
	PnlVect *St = pnl_vect_copy(this->mod_->spot_);
	PnlMat *past = pnl_mat_create(this->opt_->TimeSteps_ + 1, this->opt_->size_);

	pnl_mat_set_row(past, St, 0);

	int M = (H / this->opt_->TimeSteps_);
	int ti = 1;
	double p = 0;
	double ic = 0;
	double value = 0;

	// Affichage des valeurs du portefeuille	
	cout << " ---- \t Date n° \t PortFolio Value " << endl;
	// Initialisation du portefeuille
	this->price(p, ic);
	this->delta(past, 0, delta_cour);
	value = p - pnl_vect_scalar_prod(delta_cour, St);
	pnl_vect_set(V, 0, value);
	cout << " ---- \t " << 0 << " \t \t" << GET(V, 0) << endl;
	// Calcul des Vi
	for (int i = 1; i<H; i++)
	{
		pnl_mat_get_row(St, marketPath, i);
		pnl_mat_set_row(past, St, ti);
		if ((i % M) == 0)
			ti++;
		pnl_vect_clone(delta_prec, delta_cour);
		this->delta(past, i*this->opt_->T_ / H, delta_cour);
		pnl_vect_minus_vect(delta_prec, delta_cour);
		value = GET(V, i - 1)*exp(this->mod_->r_*this->opt_->T_ / H) + pnl_vect_scalar_prod(delta_prec, St);
		pnl_vect_set(V, i, value);
		cout << " ---- \t " << i << " \t \t" << GET(V, i) << endl;
	}
	// Calcul de VH
	pnl_mat_get_row(St, marketPath, H);
	value = GET(V, H - 1)*exp(this->mod_->r_*this->opt_->T_ / H);
	pnl_vect_set(V, H, value);
	cout << " ---- \t " << H << " \t \t" << GET(V, H) << "\t Valeur avant livraison" << endl;
	// calcul P&L
	double payoff = this->opt_->payoff(past);
	PL = GET(V, H) + pnl_vect_scalar_prod(delta_cour, St) - payoff;
	cout << " " << endl;
	cout << "  ---- Prix de l'option en 0 = " << p << endl;
	cout << "  ---- Erreur de couverture relative en % = " << (PL / p) * 100 << endl;
	// memory free
	pnl_vect_free(&St);
	pnl_vect_free(&delta_cour);
	pnl_vect_free(&delta_prec);
	pnl_mat_free(&past);
}

void MonteCarlo::hedge(PnlVect *V, double &PL, int H)
{
	PnlMat *marketPath = pnl_mat_create_from_zero(H + 1, this->opt_->size_);
	this->mod_->asset(marketPath, this->opt_->T_, H, this->rng);
	this->hedge(V, PL, H, marketPath);
	pnl_mat_free(&marketPath);
}
