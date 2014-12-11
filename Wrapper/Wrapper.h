// Wrapper.h
#pragma once
#include "pricer.h"
using namespace System;

namespace Wrapper {

	public ref class WrapperClass
	{
		// TODO: Add your methods for this class here.
		private:
			double confidenceInterval;
			double prix;
		public:
			WrapperClass() { confidenceInterval = prix = 0; };
			void getPrice(int sampleNb, double T, double S0, double K, double sigma, double r);
			double getPrice() { return prix; };
			double getIC() { return confidenceInterval; };
	};
}
