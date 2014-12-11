// This is the main DLL file.

#include "stdafx.h"

#include "Wrapper.h"
using namespace pricer;
	namespace Wrapper {
		void WrapperClass::getPrice(int sampleNb, double T, double S0, double K, double sigma, double r) {
			double ic, px;
			price(ic, px, sampleNb, T, S0, K, sigma, r);
			this->confidenceInterval = ic;
			this->prix = px;
		}
}