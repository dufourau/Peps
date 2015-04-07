#include <iostream>
#include "vasicek.h"

using namespace std;

Vasicek::Vasicek(const PnlVect *histValues) {
	this->histValues_ = pnl_vect_copy(histValues);
}

Vasicek::~Vasicek() {
	pnl_vect_free(&histValues_);
}

void Vasicek::buildSumsUtils(const PnlVect *histShortRate, double &Rx, double &Ry, double &Rxx, double &Ryy, double &Rxy) {
	for (int i = 1; i < histShortRate->size - 1; i++) {
		double tmp = GET(histShortRate, i);
		Rx += tmp;
		Rxx += tmp * tmp;
		Rxy += tmp * GET(histShortRate, i - 1);
	}
	double tmp = GET(histShortRate, 0), tmp2 = GET(histShortRate, histShortRate->size - 1);
	Ry = Rx;
	Ryy = Rxx;
	Rx += tmp;
	Ry += tmp2;
	Rxx += tmp * tmp;
	Ryy += tmp2 * tmp2;
	Rxy += tmp2 * GET(histShortRate, histShortRate->size - 2);
}

void Vasicek::calibrateParametersWithAttributeHistLeastSquare() {
	double Sx = 0., Sy = 0., Sxx = 0., Syy = 0., Sxy = 0.;
	buildSumsUtils(this->histValues_, Sx, Sy, Sxx, Syy, Sxy);
	int n = this->histValues_->size - 1;
	double a = (n*Sxy - Sx*Sy) / (n*Sxx - Sx*Sx);
	double b = (Sy - a*Sx) / n;
	double sd = sqrt((n*Syy - Sy*Sy - a*(n*Sxy - Sx*Sy)) / n / (n - 2));

	reversionSpeed_ = -log(a) / historyRateTimeStep;
	longTermMean_ = b / (1 - a);
	vol_ = sd * sqrt(-2 * log(a) / historyRateTimeStep / (1 - a*a));
	initPoint_ = GET(this->histValues_, n);
}

void Vasicek::calibrateParametersLeastSquare(const PnlVect *histShortRate, double historyRateTimeStep) {
	//double sum = pnl_vect_sum(histShortRate);
	//double Sx = sum - GET(histShortRate, histShortRate->size - 1), Sy = sum - GET(histShortRate, 0);
	double Sx = 0., Sy = 0., Sxx = 0., Syy = 0., Sxy = 0.;
	buildSumsUtils(histShortRate, Sx, Sy, Sxx, Syy, Sxy);
	int n = histShortRate->size - 1;
	double a = (n*Sxy - Sx*Sy) / (n*Sxx - Sx*Sx);
	double b = (Sy - a*Sx) / n;
	double sd = sqrt((n*Syy - Sy*Sy - a*(n*Sxy - Sx*Sy)) / n / (n - 2));

	reversionSpeed_ = -log(a) / historyRateTimeStep;
	longTermMean_ = b / (1 - a);
	vol_ = sd * sqrt(-2 * log(a) / historyRateTimeStep / (1 - a*a));
}


void Vasicek::calibrateParametersMaxLikelihood(const PnlVect *histShortRate, double historyRateTimeStep) {
	double Sx = 0., Sy = 0., Sxx = 0., Syy = 0., Sxy = 0.;
	buildSumsUtils(histShortRate, Sx, Sy, Sxx, Syy, Sxy);
	int n = histShortRate->size - 1;
	longTermMean_ = (Sy*Sxx - Sx*Sxy) / (n*(Sxx - Sxy) - (Sx * Sx - Sx*Sy));
	reversionSpeed_ = -log((Sxy - longTermMean_*Sx - longTermMean_*Sy + n*longTermMean_ *longTermMean_) / (Sxx - 2 * longTermMean_*Sx + n*longTermMean_ *longTermMean_)) / historyRateTimeStep;
	double a = exp(-reversionSpeed_ * historyRateTimeStep);
	double sigmah2 = (Syy - 2 * a*Sxy + a * a * Sxx - 2 * longTermMean_*(1 - a)*(Sy - a*Sx) + n*longTermMean_*longTermMean_ * (1 - a) * (1 - a)) / n;
	vol_ = sqrt(sigmah2 * 2 * reversionSpeed_ / (1 - a * a));
}


void Vasicek::generateDiscountFactor(double t, int N, double T, PnlRng *rng) {
	if (reversionSpeed_ == 0 || longTermMean_ == 0 || vol_ == 0)
		throw std::logic_error("La calibration du modèle n'a pas été effectuée");
	double m = longTermMean_ * (T - t) + (longTermMean_ - initPoint_) * (1 - exp(-reversionSpeed_ * (T - t))) / reversionSpeed_;
	double sigma = sqrt(-vol_ * vol_  * (1 - exp(-reversionSpeed_ * (T - t))) * (1 - exp(-reversionSpeed_ * (T - t))) / (2 * reversionSpeed_ * reversionSpeed_ * reversionSpeed_)
		+ vol_ * vol_ * (T - t - ((1 - exp(-reversionSpeed_ * (T - t))) / reversionSpeed_)) / (reversionSpeed_ * reversionSpeed_));
	this->discount_ = m + sigma * pnl_rng_normal(rng);
}

//Meth 2 plus simple: (Paramètres différents!! le t n'est pas le mm que pr l'autre version)
double Vasicek::getDiscountFactor(double t, int N, double T,
	PnlRng *rng) {
	if (reversionSpeed_ == 0 || longTermMean_ == 0 || vol_ == 0)
		throw std::logic_error("La calibration du modèle n'a pas été effectuée");

	double m = longTermMean_ * (T - t) + (initPoint_ - longTermMean_) * (1 - exp(-reversionSpeed_ * (T - t))) / reversionSpeed_;
	double sigma = sqrt(abs(-vol_ * vol_  * (1 - exp(-reversionSpeed_ * (T - t))) * (1 - exp(-reversionSpeed_ * (T - t))) / (2 * reversionSpeed_ * reversionSpeed_ * reversionSpeed_)
		+ vol_ * vol_ * (T - t - ((1 - exp(-reversionSpeed_ * (T - t))) / reversionSpeed_)) / (reversionSpeed_ * reversionSpeed_)));

	return m + sigma * pnl_rng_normal(rng);
}

void Vasicek::simuRatePath(PnlVect *path, double lastPastIndex, int N, double T,
	PnlRng *rng) {
	double step = T / ((double)N);
	double firstTerm = exp(-reversionSpeed_ * step);
	double secondTerm = longTermMean_*(1 - firstTerm);
	double thirdTerm = vol_ * sqrt((1 - firstTerm*firstTerm) / (2 * reversionSpeed_));
	for (int i = 1; i < N + 1 - lastPastIndex; i++) {
		LET(path, i) = GET(path, i - 1)*firstTerm
			+ secondTerm + thirdTerm * pnl_rng_normal(rng);
	}

}
void Vasicek::priceZeroCoupon(double& price, double t, double T)
{
	double m = longTermMean_ * (T - t) + (initPoint_ - longTermMean_) * (1 - exp(-reversionSpeed_ * (T - t))) / reversionSpeed_;
	double sigma = sqrt(-vol_ * vol_  * (1 - exp(-reversionSpeed_ * (T - t))) * (1 - exp(-reversionSpeed_ * (T - t))) / (2 * reversionSpeed_ * reversionSpeed_ * reversionSpeed_)
		+ vol_ * vol_ * (T - t - ((1 - exp(-reversionSpeed_ * (T - t))) / reversionSpeed_)) / (reversionSpeed_ * reversionSpeed_));
	price = exp(-m + sigma / 2);
}

double Vasicek::getA(double t, double T)
{
	double a = reversionSpeed_;
	return (1 - exp(-a*(T - t))) / a;
}

double Vasicek::getIntA(double t, double T) // integrale de A
{
	double a = reversionSpeed_;
	return (T - t) / a + (1 - exp(-a*(T - t))) / (a*a);
}

double Vasicek::getIntA2(double t, double T) // integrale de A*A
{
	double a = reversionSpeed_;
	return (2 * a*(T - t) + 4 * exp(-a*(T - t)) - exp(-2 * a*(T - t)) - 3) / (2 * a*a*a);
}

double Vasicek::getIntAAf(const IRate* rate, double t, double T)// integrale de A * Af d un autre taux
{
	double a = reversionSpeed_;
	double af = rate->reversionSpeed_;
	return (T - t - ((1 - exp(-a*(T - t))) / a) - ((1 - exp(-af*(T - t))) / af) + ((1 - exp(-(a + af)*(T - t))) / (a + af))) / (a*af);

}