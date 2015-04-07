#pragma once
#define DLLEXP   __declspec( dllexport )
namespace Computations{

	DLLEXP void testmethod();

	DLLEXP void compute_price(double &ic, double &prix, double *stockPrices, double *interestRates,
		double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep);

	DLLEXP void compute_price(double &ic, double &prix, double t, double* past, double *stockPrices, double *interestRates,
		double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep);
	DLLEXP void compute_price_stoch(double &ic, double &prix, double *stockPrices, double *interestRates,
		double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep, double *histEur, double *histChf, double * histGbp, double * histJpy, double * histUsd);
	
	DLLEXP void compute_price_Stoch(double &ic, double &prix, double t, double *past, double *stockPrices, double *interestRates,
		double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep, double *histEur, double *histChf, double * histGbp, double * histJpy, double * histUsd);
	
	DLLEXP void compute_delta(double *delta, double *stockPrices, double *interestRates,
		double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep);

	DLLEXP void compute_delta(double *delta, double t, double *past, double *stockPrices, double *interestRates,
		double *stockToFxIndex, int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep);

	DLLEXP void compute_hedge(double *V, double *ptfV, double *stockPrices, double *interestRates, double * stockToFxIndex,
		int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast,
		double finiteDifferenceStep, double H);
}