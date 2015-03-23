#include "Computations.h"
#include "mc.h"
#include "iostream"
#include "ctime"
#include "pnl/pnl_random.h"
using namespace std;


void Computations::compute_price(double &ic, double &prix, int option_size, double* dividend, double* curr, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples,int sizeAsset)
{

	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, option_size, optionType_, r, rho, curr, dividend, sigma, spot, trend, samples, sizeAsset);
	mc->price(prix,ic);
	delete mc;
}

void Computations::compute_price(double &ic, double &prix,double t,double *past, int option_size, double* dividend, double* curr, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples, int sizeAsset)
{

	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, option_size, optionType_, r, rho, curr, dividend, sigma, spot, trend, samples, sizeAsset);
	//m rows and n columns
	double step = maturity / timeSteps;
	//double dt = t / step;
	int m;
	//valeur exacte
	if (floor(t)==t){
		m = floor(t);
	}
	else{
		m = floor(t)+1;
	}
	//We have to include the spot price
	m++;
	//PnlMat* pastMat=pnl_mat_create_from_zero(timeSteps+1, option_size);
	PnlMat* pastMat = pnl_mat_create_from_ptr(m,option_size,past);
	mc->price(pastMat,t,prix, ic);
	delete mc;
}


void Computations::compute_delta(double *delta, double *ic, int option_size, double* dividend, double* curr, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples, int sizeAsset)
{
	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, option_size, optionType_, r, rho, curr, dividend, sigma, spot, trend, samples, sizeAsset);
	PnlVect* deltaVect = pnl_vect_new();
	deltaVect = pnl_vect_create(option_size);
	//PnlVect* icVect = pnl_vect_new();
	//icVect = pnl_vect_create(option_size);
	PnlMat* past = pnl_mat_create(1, option_size);
	PnlVect* spot_ = pnl_vect_create_from_ptr(option_size, spot);
	pnl_mat_set_row(past, spot_,0);
	mc->delta(past,0.0,deltaVect);
	for (int i = 0; i < option_size; i++){
		delta[i]= GET(deltaVect,i);
		//ic[i]= GET(icVect,i);
	}
	delete mc;
	pnl_vect_free(&deltaVect);
	pnl_mat_free(&past);
	pnl_vect_free(&spot_);
}

void Computations::compute_delta(double *delta, double *ic, double t, double *past, int option_size, double* dividend, double* curr, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples, int sizeAsset)
{
	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, option_size, optionType_, r, rho, curr, dividend, sigma, spot, trend, samples, sizeAsset);
	PnlVect* deltaVect = pnl_vect_new();
	deltaVect = pnl_vect_create(option_size);
	//PnlVect* icVect = pnl_vect_new();
	//icVect = pnl_vect_create(option_size);
	int m;
	//valeur exacte
	if (floor(t) == t){
		m = floor(t);
	}
	else{
		m = floor(t) + 1;
	}
	//We have to include the spot price
	m++;
	//PnlMat* pastMat=pnl_mat_create_from_zero(timeSteps+1, option_size);
	PnlMat* pastMat = pnl_mat_create_from_ptr(m, option_size, past);

	PnlVect* spot_ = pnl_vect_create_from_ptr(option_size, spot);
	
	mc->delta(pastMat, t, deltaVect);
	for (int i = 0; i < option_size; i++){
		delta[i] = GET(deltaVect, i);
		//ic[i]= GET(icVect,i);
	}
	delete mc;
	pnl_vect_free(&deltaVect);
	pnl_mat_free(&pastMat);
	pnl_vect_free(&spot_);
}


void Computations::compute_hedge(double &PL, int option_size, double* dividend, double* curr, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples, int sizeAsset){
	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, option_size, optionType_, r, rho, curr, dividend , sigma, spot, trend, samples, sizeAsset);
	PnlVect *V = pnl_vect_create_from_zero(H + 1);
	PL = 0;
	//temps = MPI_Wtime();
	mc->hedge(V, PL, H);
	pnl_vect_free(&V);

}
