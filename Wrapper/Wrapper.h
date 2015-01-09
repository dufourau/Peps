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
			int option_size;
			double r;
			double rho; 
			double h; 
			int H; 
			double maturity; 
			int timeSteps; 
			double strike; 
			int samples;
			array<double> ^spot;
			array<double> ^sigma;
			array<double> ^trend;
			array<double> ^coeff;
		public:
			//Temp constructor for a basket option
			WrapperClass() { 
				confidenceInterval = prix = 0; 
				PL = 0;
				option_size = 5;
				strike = 100;
				maturity = 10;
				r = 0.04879;
				rho = 0;
				timeSteps = 10;
				h = 0.1;
				H = timeSteps*2;
				samples = 1000; 
				spot = gcnew array<double>(option_size);
				for (int i = 0; i < option_size; i++){
					spot[i] = 100;
				}
				sigma = gcnew array<double>(option_size);
				for (int i = 0; i < option_size; i++){
					sigma[i] = 0.3;
				}
				trend = gcnew array<double>(option_size);
				for (int i = 0; i < option_size; i++){
					trend[i] = 0.06;
				}
				coeff = gcnew array<double>(option_size);
				for (int i = 0; i < option_size; i++){
					coeff[i] = 0.025;
				}
				delta = gcnew array<double>(option_size);
				ic = gcnew array<double>(option_size);
			
			
			};
			void computePrice();
			void computeDelta();
			void computeHedge();
			double getPrice() { return prix; };
			double getIC() { return confidenceInterval; };
			double getPL() { return PL; };
			int getSize(){ return option_size; };
			array<double> ^getDelta(){ return delta; };
			array<double> ^getDeltaIC(){ return ic; };
	};
}
