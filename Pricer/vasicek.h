#ifndef _VASICEK_H
#define _VASICEK_H

#include "pnl/pnl_vector.h"
#include "pnl/pnl_matrix.h"
#include <cmath>
#include "irate.h"

/// \brief Classe Option PERFORMANCE
class Vasicek : public IRate
{
public:



	double historyRateTimeStep = 1. / 252.;



	Vasicek(const PnlVect *histValues);
	virtual ~Vasicek();
	void buildSumsUtils(const PnlVect *histShortRate, double &Rx, double &Ry, double &Rxx, double &Ryy, double &Rxy);
	void calibrateParametersLeastSquare(const PnlVect *histShortRate, double historyRateTimeStep);
	void calibrateParametersMaxLikelihood(const PnlVect *histShortRate, double historyRateTimeStep);
	void priceZeroCoupon(double& price, double t, double maturity);

	double getA(double t, double maturity);

	double getIntA(double t, double maturity); // integrale de A

	double getIntA2(double t, double maturity); // integrale de A*A

	double getIntAAf(const IRate* rate, double t, double maturity);// integrale de A * Af d un autre taux

	void calibrateParametersWithAttributeHistLeastSquare();

	double getDiscountFactor(double t, int N, double T,
		PnlRng *rng);

	void generateDiscountFactor(double t, int N, double T, PnlRng *rng);

private:
	void simuRatePath(PnlVect *path, double t, int N, double T,
		PnlRng *rng);
};


#endif /* _VASICEK_H */

