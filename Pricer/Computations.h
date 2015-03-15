#pragma once
#define DLLEXP   __declspec( dllexport )
namespace Computations{
	DLLEXP void compute_price(double &ic, double &prix, int option_size, double* dividend, double* curr, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples);

	DLLEXP void compute_delta(double *deltaVect, double *icVect, int option_size, double *spot, double* dividend, double* curr, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples);
	
	DLLEXP void compute_hedge(double &PL, int option_size, double* dividend, double* curr, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples);
}