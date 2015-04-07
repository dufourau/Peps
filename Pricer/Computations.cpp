#include "Computations.h"
#include "mc.h"
#include "iostream"
#include "ctime"
#include "pnl/pnl_random.h"
#include "vasicek.h"
using namespace std;


void Computations::testmethod() {

}

void Computations::compute_price(double &ic, double &prix, double *stockPrices, double *interestRates,
	double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep)
{
	PnlMat* stocksPx = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);

	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, assetNb + fxNb, interestRates[0], stockToFxIndex,
		interestRates + 1, stocksPx, mcSamples, assetNb, dimStockPast, finiteDifferenceStep);
	PnlMat* marketPath = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);
	mc->mod_->calibrate(marketPath, 1 / 252.);
	mc->price(prix, ic);
	pnl_mat_free(&marketPath);
	pnl_mat_free(&stocksPx);
	delete mc;
}

void Computations::compute_price(double &ic, double &prix, double t, double *past, double *stockPrices, double *interestRates,
	double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep)
{
	PnlMat* stocksPx = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);

	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, assetNb + fxNb, interestRates[0], stockToFxIndex,
		interestRates + 1, stocksPx, mcSamples, assetNb, dimStockPast, finiteDifferenceStep);
	PnlMat* marketPath = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);
	int m;
	if (floor(t)==t){
		m = floor(t)+1;
	}
	else{
		m = floor(t)+2;
	}
	PnlMat* pastMat = pnl_mat_create_from_ptr(m, assetNb + fxNb, past);

	mc->mod_->calibrate(marketPath, 1 / 252.);
	mc->price(pastMat,t,prix,ic);
	pnl_mat_free(&marketPath);
	pnl_mat_free(&stocksPx);
	delete mc;
}

void Computations::compute_price_stoch(double &ic, double &prix, double *stockPrices, double *interestRates,
	double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep, double *histEur, double *histChf, double * histGbp, double * histJpy, double * histUsd )
{
	PnlMat* stocksPx = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);

	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, assetNb + fxNb, interestRates[0], stockToFxIndex,
		interestRates + 1, stocksPx, mcSamples, assetNb, dimStockPast, finiteDifferenceStep);
	PnlVect *histRates = pnl_vect_create_from_ptr(47,histEur);
	Vasicek v(histRates);
	mc->mod_->stochIRates_.push_back(&v);

	PnlVect *histRates1 = pnl_vect_create_from_ptr(47, histChf);
	Vasicek v1(histRates1);
	mc->mod_->stochIRates_.push_back(&v1);

	PnlVect *histRates2 = pnl_vect_create_from_ptr(47, histGbp);
	Vasicek v2(histRates2);
	mc->mod_->stochIRates_.push_back(&v2);

	PnlVect *histRates3 = pnl_vect_create_from_ptr(47, histJpy);
	Vasicek v3(histRates3);
	mc->mod_->stochIRates_.push_back(&v3);

	PnlVect *histRates4 = pnl_vect_create_from_ptr(47, histUsd);
	Vasicek v4(histRates4);
	mc->mod_->stochIRates_.push_back(&v4);
	PnlMat* marketPath = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);
	mc->mod_->calibrate(marketPath, 1 / 252.);
	mc->priceStoch(prix, ic);
	pnl_mat_free(&marketPath);
	pnl_mat_free(&stocksPx);
	delete mc;
}

void Computations::compute_price_Stoch(double &ic, double &prix, double t, double *past, double *stockPrices, double *interestRates,
	double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep, double *histEur, double *histChf, double * histGbp, double * histJpy, double * histUsd)
{
	PnlMat* stocksPx = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);

	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, assetNb + fxNb, interestRates[0], stockToFxIndex,
		interestRates + 1, stocksPx, mcSamples, assetNb, dimStockPast, finiteDifferenceStep);
	PnlMat* marketPath = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);
	int m;
	if (floor(t) == t){
		m = floor(t) + 1;
	}
	else{
		m = floor(t) + 2;
	}
	PnlMat* pastMat = pnl_mat_create_from_ptr(m, assetNb + fxNb, past);
	PnlVect *histRates = pnl_vect_create_from_ptr(47, histEur);
	pnl_vect_div_scalar(histRates, 100);
	Vasicek v(histRates);
	mc->mod_->stochIRates_.push_back(&v);

	PnlVect *histRates1 = pnl_vect_create_from_ptr(47, histChf);
	pnl_vect_div_scalar(histRates1, 100);
	Vasicek v1(histRates1);
	mc->mod_->stochIRates_.push_back(&v1);

	PnlVect *histRates2 = pnl_vect_create_from_ptr(47, histGbp);
	pnl_vect_div_scalar(histRates2, 100);
	Vasicek v2(histRates2);
	mc->mod_->stochIRates_.push_back(&v2);

	PnlVect *histRates3 = pnl_vect_create_from_ptr(47, histJpy);
	pnl_vect_div_scalar(histRates3, 100);
	Vasicek v3(histRates3);
	mc->mod_->stochIRates_.push_back(&v3);

	PnlVect *histRates4 = pnl_vect_create_from_ptr(47, histUsd);
	pnl_vect_div_scalar(histRates4, 100);
	Vasicek v4(histRates4);

	mc->mod_->calibrate(marketPath, 1 / 252.);
	mc->priceStoch(pastMat, t, prix, ic);
	pnl_mat_free(&marketPath);
	pnl_mat_free(&stocksPx);
	delete mc;
}

void Computations::compute_delta(double *delta, double *stockPrices, double *interestRates,
	double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep)
{
	PnlMat* stocksPx = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);

	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, assetNb + fxNb, interestRates[0], stockToFxIndex,
		interestRates + 1, stocksPx, mcSamples, assetNb, dimStockPast, finiteDifferenceStep);
	PnlMat* marketPath = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);
	mc->mod_->calibrate(marketPath, 1 / 252.);

	PnlVect* deltaVect = pnl_vect_create(assetNb + fxNb);
	PnlMat* past = pnl_mat_create(1, assetNb + fxNb);
	PnlVect* spot_ = pnl_vect_create(assetNb + fxNb);

	pnl_mat_get_row(spot_, stocksPx, dimStockPast - 1);
	pnl_mat_set_row(past, spot_, 0);
	mc->delta(past, 0.0, deltaVect);
	for (int i = 0; i < assetNb + fxNb; i++){
		delta[i] = GET(deltaVect, i);
		//ic[i]= GET(icVect,i);
	}
	delete mc;
	pnl_mat_free(&marketPath);
	pnl_vect_free(&deltaVect);
	pnl_mat_free(&past);
	pnl_vect_free(&spot_);
}

void Computations::compute_delta(double *delta, double t, double *past, double *stockPrices, double *interestRates,
	double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep)
{
	PnlMat* stocksPx = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);

	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, assetNb + fxNb, interestRates[0], stockToFxIndex,
		interestRates + 1, stocksPx, mcSamples, assetNb, dimStockPast, finiteDifferenceStep);
	PnlMat* marketPath = pnl_mat_create_from_ptr(dimStockPast, assetNb + fxNb, stockPrices);
	mc->mod_->calibrate(marketPath, 1 / 252.);

	PnlVect* deltaVect = pnl_vect_create(assetNb + fxNb);
	int m;
	if (floor(t) == t){
		m = floor(t)+2;
	}
	else{
		m = floor(t) + 3;
	}
	PnlMat* pastMat = pnl_mat_create_from_ptr(m, assetNb + fxNb, past);
	PnlVect* spot_ = pnl_vect_create(assetNb + fxNb);

	pnl_mat_get_row(spot_, stocksPx, dimStockPast - 1);
	
	mc->delta(pastMat, t, deltaVect);
	for (int i = 0; i < assetNb + fxNb; i++){
		delta[i] = GET(deltaVect, i);
		//ic[i]= GET(icVect,i);
	}
	delete mc;
	pnl_mat_free(&marketPath);
	pnl_vect_free(&deltaVect);
	pnl_mat_free(&pastMat);
	pnl_vect_free(&spot_);
	
}



void Computations::compute_hedge(double *V, double *ptfV, double *stockPrices, double *interestRates, double * stockToFxIndex,
	int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast,
	double finiteDifferenceStep, double H){
	int calibrationValuesNb = dimStockPast / 10;
	PnlMat *calibratePath = pnl_mat_create(calibrationValuesNb, assetNb + fxNb);
	PnlMat* marketPath = pnl_mat_create(dimStockPast - calibrationValuesNb, assetNb + fxNb);
	double PL = 0;
	int k = 0;
	for (int i = 0; i < calibrationValuesNb; i++) {
		for (int j = 0; j < assetNb + fxNb; j++) {
			MLET(calibratePath, i, j) = stockPrices[k++];
		}
	}
	k = 0;
	for (int i = calibrationValuesNb; i < dimStockPast; i++) {
		for (int j = 0; j < assetNb + fxNb; j++) {
			MLET(marketPath, i, j) = stockPrices[k++];
		}
	}

	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, assetNb + fxNb, interestRates[0], stockToFxIndex,
		interestRates + 1, calibratePath, mcSamples, assetNb, calibrationValuesNb, finiteDifferenceStep);

	mc->mod_->calibrate(calibratePath, 1 / 252.);

	PnlVect *cashValues = pnl_vect_create(H + 1);
	PnlVect *ptfValues = pnl_vect_create(H + 1);
	//mc->hedge(cashValues, ptfValues, PL, H, marketPath);
	//PL = 0;
	////temps = MPI_Wtime();
	//mc->hedge(V, PL, H);
	pnl_vect_free(&ptfValues);
	pnl_vect_free(&cashValues);
	pnl_mat_free(&marketPath);
	pnl_mat_free(&calibratePath);
}
