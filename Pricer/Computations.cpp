#include "Computations.h"
#include "mc.h"
#include "iostream"
#include "ctime"
#include "pnl/pnl_random.h"
using namespace std;


void Computations::compute_price(double &ic, double &prix, int option_size, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples)
{

	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps,  option_size,optionType_,r, rho,sigma,spot, trend,samples);
	mc->price(prix,ic);
	delete mc;
}


void Computations::compute_delta(double *delta, double *ic, int option_size, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples)
{
	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, option_size, optionType_, r, rho, sigma, spot, trend, samples);
	PnlVect* deltaVect = pnl_vect_new();
	deltaVect = pnl_vect_create(option_size);
	//PnlVect* icVect = pnl_vect_new();
	//icVect = pnl_vect_create(option_size);
	PnlMat* past = pnl_mat_create(1, option_size);
	PnlVect* spot_ = pnl_vect_create_from_ptr(option_size, spot);
	pnl_mat_set_row(past, spot_,0);
	mc->delta(past,0,deltaVect);
	for (int i = 0; i < option_size; i++){
		delta[i]= GET(deltaVect,i);
		//ic[i]= GET(icVect,i);
	}
	delete mc;
	pnl_vect_free(&deltaVect);
	pnl_mat_free(&past);
	pnl_vect_free(&spot_);
}

void Computations::compute_hedge(double &PL, int option_size, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples){
	int optionType_;
	MonteCarlo* mc = new MonteCarlo(maturity, timeSteps, option_size, optionType_, r, rho, sigma, spot, trend, samples);
	PnlVect *V = pnl_vect_create_from_zero(H + 1);
	PL = 0;
	//temps = MPI_Wtime();
	mc->hedge(V, PL, H);
	pnl_vect_free(&V);

}
