// This is the main DLL file.
#include "stdafx.h"

#include "Wrapper.h"
using namespace Computations;
	namespace Wrapper {
		void WrapperClass::computePrice(){
			double ic, px;
			pin_ptr<double> pSpot = &spot[0];
			pin_ptr<double> pSigma = &sigma[0];
			pin_ptr<double> pTrend = &trend[0];
			pin_ptr<double> pCoeff = &coeff[0];
			compute_price(ic, px,option_size, pSpot, pSigma, pTrend, r, rho ,h , H, maturity,timeSteps,strike, pCoeff,samples);
			this->confidenceInterval = ic;
			this->prix = px;
			
		}
		void WrapperClass::computeDelta(){
			pin_ptr<double> pDeltaIc = &ic[0];
			pin_ptr<double> pDelta = &delta[0];
			pin_ptr<double> pSpot = &spot[0];
			pin_ptr<double> pSigma = &sigma[0];
			pin_ptr<double> pTrend = &trend[0];
			pin_ptr<double> pCoeff = &coeff[0];
			compute_delta(pDelta, pDeltaIc, option_size, pSpot, pSigma, pTrend, r, rho, h, H, maturity, timeSteps, strike, pCoeff, samples);
		}

		void WrapperClass::computeHedge(){
			double Pl;
			pin_ptr<double> pSpot = &spot[0];
			pin_ptr<double> pSigma = &sigma[0];
			pin_ptr<double> pTrend = &trend[0];
			pin_ptr<double> pCoeff = &coeff[0];
			compute_hedge(Pl, option_size, pSpot, pSigma, pTrend, r, rho, h, H, maturity, timeSteps, strike, pCoeff, samples);
			this->PL = Pl;
		}
}