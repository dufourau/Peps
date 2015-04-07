// Wrapper.h
#pragma once
#include "Computations.h"
using namespace System;

namespace Wrapper {

	public ref class WrapperClass
	{
	private:
		double confidenceInterval;
		double prix;
		double PL;
		array<double> ^delta;
		array<double> ^ic;


		array<double> ^curr;
		array<double> ^past;
	public:

		array<double> ^ptfValues;
		array<double> ^cashValues;
		//Temp constructor for a basket option
		WrapperClass() {
			confidenceInterval = prix = 0;
			PL = 0;
		};
		
		void computePrice(array<double>^ stockPrices, array<double>^ interestRates, array<double>^ stockToFxIndex,
			int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int nbDatesStockPrices, double finiteDifferenceStep);
		void computePrice(double t, array<double>^pastPrices, array<double>^ stockPrices, array<double>^ interestRates, array<double>^ stockToFxIndex,
			int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int nbDatesStockPrices, double finiteDifferenceStep);
		void computePriceStoch(array<double>^ stockPrices, array<double>^ interestRates, array<double>^ stockToFxIndex,
			int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int nbDatesStockPrices, double finiteDifferenceStep, array<double>^ histEur, array<double>^ histChf, array<double>^ histGbp, array<double>^ histJpy, array<double>^ histUsd);
		void computeDelta(array<double>^ stockPrices, array<double>^ interestRates, array<double>^ stockToFxIndex,
			int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int nbDatesStockPrices, double finiteDifferenceStep);
		void computeDelta(double t, array<double>^pastPrices, array<double>^ stockPrices, array<double>^ interestRates, array<double>^ stockToFxIndex,
			int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int nbDatesStockPrices, double finiteDifferenceStep);
		void computeHedge(array<double>^ stockPrices, array<double>^ interestRates, array<double>^ stockToFxIndex,
			int assetNb, int fxNb, double maturity, int mcSamples, int timeSteps, int dimStockPast, double finiteDifferenceStep, double H);
		double getPrice() { return prix; };
		double getIC() { return confidenceInterval; };
		double getPL() { return PL; };
		array<double> ^getDelta(){ return delta; };
		array<double> ^getDeltaIC(){ return ic; };
	};
}
