#pragma once
#define DLLEXP   __declspec( dllexport )
namespace pricer{
	DLLEXP void price(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r);
}