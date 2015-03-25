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
			int nb_curr;
			double r;
			double rho; 
			double h; 
			int H; 
			int pastIndex;
			int histIndex;
			double maturity; 
			int timeSteps; 
			double strike; 
			int samples;
			array<double> ^dividend;
			array<double> ^curr;
			array<double> ^spot;
			array<double> ^sigma;
			array<double> ^trend;
			array<double> ^coeff;
			array<double> ^past;
			array<double> ^historicalStockPrices;
		public:
			//Temp constructor for a basket option
			WrapperClass() { 
				confidenceInterval = prix = 0; 
				PL = 0;
				option_size = 20;
				nb_curr = 4;
				strike = 100;
				maturity = 10;
				//Euribor 3 months
				r = 0.05;
				rho = 0;
				timeSteps = 10;
				h = 0.1;
				H = 250*maturity;
				samples = 1000; 
				spot = gcnew array<double>(option_size+nb_curr);
				for (int i = 0; i < option_size; i++){
					spot[i] = 100;
				}
				spot[8] = 1.4;
				spot[9] = 0.95;
				spot[10] = 0.9;
				sigma = gcnew array<double>(option_size+nb_curr);
				for (int i = 0; i < option_size+nb_curr; i++){
					sigma[i] = 0.3;
				}
				trend = gcnew array<double>(option_size+nb_curr);
				for (int i = 0; i < option_size + nb_curr; i++){
					trend[i] = 0.1;
				}
				coeff = gcnew array<double>(option_size+nb_curr);
				for (int i = 0; i < option_size + nb_curr; i++){
					coeff[i] = 0.025;
				}
				dividend = gcnew array<double>(option_size+nb_curr);
				for (int i = 0; i < option_size; i++){
					dividend[i] = 0;
				}
				
				//Livre
				dividend[option_size] = 0.04;
				//Dollars
				dividend[option_size+1] = 0.035;
				//CHF
				dividend[option_size + 2] = 0.045;
				//Yen
				dividend[option_size + 3] = 0.03;
				curr = gcnew array<double>(option_size);
				for (int i = 0; i < option_size; i++){
					curr[i] = -1;
				}
				
				curr[2] = 20;
				curr[3] = 20;
				curr[4] = 20;
				curr[5] = 21;
				curr[6] = 21;
				curr[7] = 22;
				curr[9] = 20;
				curr[10] = 20;
				curr[11] = 21;
				curr[12] = 21;
				curr[13] = 21;
				curr[14] = 21;
				curr[15] = 21;
				curr[16] = 23;
				delta = gcnew array<double>(option_size+nb_curr);
				ic = gcnew array<double>(option_size+nb_curr);
			};

			void computePrice();
			void computePrice(double t);
			void computeDelta();
			void computeDelta(double t);
			void computeHedge();
			void computeVol();
			double getPrice() { return prix; };
			double getIC() { return confidenceInterval; };
			double getPL() { return PL; };
			int getSize(){ return option_size; };
			array<double> ^getDelta(){ return delta; };
			array<double> ^getDeltaIC(){ return ic; };

			void initPast(int nbCol){
				pastIndex = 0;
				past = gcnew array<double>((option_size + nb_curr)*nbCol);
			}

			int getOption_size(){ return option_size; };
			int getCurr_size(){ return nb_curr; };
			double getR(){ return r; };
			double getRho(){ return rho; };
			double geth(){ return h; };
			int getH(){ return H; };
			double getMaturity(){ return maturity; };
			double getStrike(){ return strike; };
			//row i col j
			void set(double val){ 
				past[pastIndex] = val; pastIndex++; 
			}
			void setSpot(double val, int index){
				spot[index] = val;
			}
			//Init matrix to calibrate the vol
			void initHistPrice(int nbCol){
				histIndex = 0;
				historicalStockPrices = gcnew array<double>((option_size + nb_curr)*nbCol);
			}
			void setHistPrice(double val){
				historicalStockPrices[histIndex] = val; histIndex++;
			}
	};
}
