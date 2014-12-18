#include "mc.h"
#include "bs.h"
#include "OptionBasket.h"

MonteCarlo::MonteCarlo(int option_size, double *spot, double *sigma, double* trend, double r, double rho, double h, int H, double maturity, int timeSteps, double strike, double* payoffCoeff, int samples)
{

	PnlVect* spotVect = pnl_vect_new();
	PnlVect* sigmaVect= pnl_vect_new();
	PnlVect* trendVect = pnl_vect_new();
	PnlVect* payoffCoeffVect = pnl_vect_new();
	spotVect= pnl_vect_create(option_size);
	sigmaVect = pnl_vect_create(option_size);
	trendVect = pnl_vect_create(option_size);
	payoffCoeffVect = pnl_vect_create(option_size);
	for (int i = 0; i < option_size; i++){
		LET(spotVect, i) = spot[i];
		LET(sigmaVect, i) = sigma[i];
		LET(trendVect, i) = trend[i];
		LET(payoffCoeffVect, i) = payoffCoeff[i];
	}
	
	this->H_ = H;
	this->h_ = h;
	this->mod_ = new BS(spotVect, sigmaVect, rho, r, option_size, trendVect);

	this->opt_ = new OptionBasket(maturity, timeSteps,option_size, strike, payoffCoeffVect);


	this->samples_= samples;

	//RNG
	this->rng = pnl_rng_create(PNL_RNG_MERSENNE);
	pnl_rng_sseed(this->rng, 0);

}


MonteCarlo::~MonteCarlo(){
	delete (this->mod_)->spot_;
	delete (this->mod_)->sigma_;
	delete this->mod_;
	pnl_rng_free(&(this->rng));
	delete this->opt_;
}

Option* MonteCarlo::createOption(){
	return NULL;
}


/**
* Calcul le prix de l'option en t=0 et la largeur de son intervalle de confinace
*/
void MonteCarlo::price(double &prix, double &ic){
	double coeffActu = exp(-(mod_->r_ * opt_->T_));
	//Matrix of assets

	//Initialize with spot
	PnlMat* path;
	path = pnl_mat_create(opt_->TimeSteps_ + 1, (this->mod_)->size_);

	//Calcul du payOff   
	double payOffOption = 0;
	double mean_payOffSquare = 0;
	double tmp;

	for (int m = 1; m <= this->samples_; m++){
		mod_->asset(path, opt_->T_, opt_->TimeSteps_, this->rng);
		tmp = opt_->payoff(path);
		payOffOption += tmp;
		mean_payOffSquare += tmp*tmp;
	}

	payOffOption = payOffOption / this->samples_;
	mean_payOffSquare = mean_payOffSquare / this->samples_;

	//Calcul du prix de l'option en t=0
	prix = coeffActu * payOffOption;

	//Free path
	pnl_mat_free(&path);

	//Calcul de la largeur de l'intervalle de confinace
	double cst = exp(-2 * (mod_->r_ * opt_->T_));

	double varEstimator = cst * (mean_payOffSquare - (payOffOption*payOffOption));


	//ic = (prix + 1.96*sqrt(varEstimator) / sqrt(this->samples_)) - (prix - 1.96*sqrt(varEstimator) / sqrt(this->samples_));
}



void MonteCarlo::freeRiskInvestedPart(PnlVect *V, double T, double &profitLoss){
	//V = pnl_vect_create(this->H_);
	PnlMat *simulMarketResult, *tempMarketResult;
	simulMarketResult = pnl_mat_create(this->H_ + 1, this->mod_->size_);

	//Simulate H+1 values from 0 to T (market values)
	mod_->simul_market(simulMarketResult, T, this->H_, this->rng);

	PnlVect* precDelta, *ecartDelta, *copydelta_i;

	//Current Time of iteration
	double tho = 0.0;

	/* Compute V_0 */
	//Compute Price
	double refprice, refic;

	this->price(refprice, refic);
	//Compute delta_0
	PnlVect* delta, *ic;
	delta = pnl_vect_create(this->mod_->size_);
	ic = pnl_vect_create(this->mod_->size_);
	this->delta(simulMarketResult, tho, delta, ic);
	pnl_vect_print(delta);

	//On r�cup�re S_0
	PnlVect *s;
	s = pnl_vect_create(this->mod_->size_);
	pnl_mat_get_row(s, simulMarketResult, 0);

	//On calcul V_0
	LET(V, 0) = refprice - pnl_vect_scalar_prod(delta, s);
	precDelta = pnl_vect_copy(delta); //on sauvergarde le delta qu'on vient de calculer


	/* Compute V_i */
	for (int i = 1; i<V->size; i++){
		//On incr�mente tho du pas de discr�tisation de la simulation de march� � savoir T/H
		tho += T / ((double) this->H_);


		//Extract the row from 0 to tho "time"
		tempMarketResult = pnl_mat_create(i + 1, this->mod_->size_);
		pnl_mat_extract_subblock(tempMarketResult, simulMarketResult, 0, i + 1, 0, this->mod_->size_);

		//Compute delta_i
		this->delta(tempMarketResult, tho, delta, ic);

		pnl_vect_print(delta);

		copydelta_i = pnl_vect_copy(delta);
		pnl_vect_minus_vect(copydelta_i, precDelta);
		pnl_mat_get_row(s, simulMarketResult, i);
		LET(V, i) = GET(V, i - 1)*exp(mod_->r_ * T / ((double) this->H_)) - pnl_vect_scalar_prod(copydelta_i, s);
		precDelta = pnl_vect_copy(delta);

		pnl_mat_free(&tempMarketResult);
	}

	profitLoss = GET(V, V->size - 1) + pnl_vect_scalar_prod(precDelta, s) - this->opt_->payoff(simulMarketResult);
	pnl_vect_free(&s);
	pnl_vect_free(&delta);
	pnl_vect_free(&ic);
	pnl_vect_free(&precDelta);

	pnl_mat_free(&simulMarketResult);
}

void MonteCarlo::delta(const PnlMat *past, double t, PnlVect *delta, PnlVect *ic){
	int nbAsset = this->opt_->size_;
	PnlMat* path_shift_up = pnl_mat_create(this->opt_->TimeSteps_ + 1, nbAsset);
	PnlMat* path_shift_down = pnl_mat_create(this->opt_->TimeSteps_ + 1, nbAsset);
	PnlMat* path = pnl_mat_create(this->opt_->TimeSteps_ + 1, nbAsset);
	PnlVect* sum = pnl_vect_create(nbAsset);
	double tstep = this->opt_->T_ / this->opt_->TimeSteps_;

	for (int j = 0; j < this->samples_; ++j){
		//Select the right asset method to call  
		if (t == 0){
			this->mod_->asset(path, this->opt_->T_, this->opt_->TimeSteps_, this->rng);
		}
		else{

			this->mod_->asset(path, t, this->opt_->TimeSteps_, this->opt_->T_, this->rng, past);

		}

		for (int i = 0; i < nbAsset; ++i){
			this->mod_->shift_asset(path_shift_up, path, i, this->h_, t, tstep);
			this->mod_->shift_asset(path_shift_down, path, i, -this->h_, t, tstep);
			LET(sum, i) = GET(sum, i) + this->opt_->payoff(path_shift_up) - this->opt_->payoff(path_shift_down);

		}
	}

	for (int i = 0; i < nbAsset; i++){
		if (t == 0){
			LET(delta, i) = GET(sum, i) * exp(-this->mod_->r_ * (this->opt_->T_ - t)) / (2.0 * this->samples_ * MGET(path, 0, i) * this->h_);
		}
		else{
			LET(delta, i) = GET(sum, i) * exp(-this->mod_->r_ * (this->opt_->T_ - t)) / (2.0 * this->samples_ * MGET(past, past->m - 1, i) * this->h_);
		}
	}

	pnl_vect_free(&sum);
	pnl_mat_free(&path);
	pnl_mat_free(&path_shift_up);
	pnl_mat_free(&path_shift_down);
}
