#include "pricer.h"
#include "mc.h"
#include "iostream"
#include "ctime"
#include "pnl/pnl_random.h"

void pricer::price(double &ic, double &prix, int nb_samples, double T,
	double S0, double K, double sigma, double r)
{
	ic = 2;
	prix = 3;
	//MonteCarlo *mc = new MonteCarlo();
	//mc->price(ic, prix);
}