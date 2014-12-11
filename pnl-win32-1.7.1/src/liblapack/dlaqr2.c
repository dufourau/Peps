/* dlaqr2.f -- translated by f2c (version 20061008).
   You must link the resulting object file with libf2c:
	on Microsoft Windows system, link with libf2c.lib;
	on Linux or Unix systems, link with .../path/to/libf2c.a -lm
	or, if you install libf2c.a in a standard place, with -lf2c -lm
	-- in that order, at the end of the command line, as in
		cc *.o -lf2c -lm
	Source for libf2c is in /netlib/f2c/libf2c.zip, e.g.,

		http://www.netlib.org/f2c/libf2c.zip
*/

#include "pnl/pnl_f2c.h"

/* Table of constant values */

static int c__1 = 1;
static int c_n1 = -1;
static double c_b12 = 0.;
static double c_b13 = 1.;
static int c_true = TRUE;

 int dlaqr2_(int *wantt, int *wantz, int *n, 
	int *ktop, int *kbot, int *nw, double *h__, int *
	ldh, int *iloz, int *ihiz, double *z__, int *ldz, 
	int *ns, int *nd, double *sr, double *si, double *
	v, int *ldv, int *nh, double *t, int *ldt, int *
	nv, double *wv, int *ldwv, double *work, int *lwork)
{
    /* System generated locals */
    int h_dim1, h_offset, t_dim1, t_offset, v_dim1, v_offset, wv_dim1, 
	    wv_offset, z_dim1, z_offset, i__1, i__2, i__3, i__4;
    double d__1, d__2, d__3, d__4, d__5, d__6;

    /* Builtin functions */
    double sqrt(double);

    /* Local variables */
    int i__, j, k;
    double s, aa, bb, cc, dd, cs, sn;
    int jw;
    double evi, evk, foo;
    int kln;
    double tau, ulp;
    int lwk1, lwk2;
    double beta;
    int kend, kcol, info, ifst, ilst, ltop, krow;
    extern  int dlarf_(char *, int *, int *, 
	    double *, int *, double *, double *, int *, 
	    double *), dgemm_(char *, char *, int *, int *
, int *, double *, double *, int *, double *, 
	    int *, double *, double *, int *);
    int bulge;
    extern  int dcopy_(int *, double *, int *, 
	    double *, int *);
    int infqr, kwtop;
    extern  int dlanv2_(double *, double *, 
	    double *, double *, double *, double *, 
	    double *, double *, double *, double *), dlabad_(
	    double *, double *);
    extern double dlamch_(char *);
    extern  int dgehrd_(int *, int *, int *, 
	    double *, int *, double *, double *, int *, 
	    int *), dlarfg_(int *, double *, double *, 
	    int *, double *), dlahqr_(int *, int *, int *, 
	     int *, int *, double *, int *, double *, 
	    double *, int *, int *, double *, int *, 
	    int *), dlacpy_(char *, int *, int *, double *, 
	    int *, double *, int *);
    double safmin;
    extern  int dlaset_(char *, int *, int *, 
	    double *, double *, double *, int *);
    double safmax;
    extern  int dtrexc_(char *, int *, double *, 
	    int *, double *, int *, int *, int *, 
	    double *, int *), dormhr_(char *, char *, int 
	    *, int *, int *, int *, double *, int *, 
	    double *, double *, int *, double *, int *, 
	    int *);
    int sorted;
    double smlnum;
    int lwkopt;


/*  -- LAPACK auxiliary routine (version 3.2.1)                        -- */
/*     Univ. of Tennessee, Univ. of California Berkeley, Univ. of Colorado Denver and NAG Ltd.. */
/*  -- April 2009                                                      -- */

/*     .. Scalar Arguments .. */
/*     .. */
/*     .. Array Arguments .. */
/*     .. */

/*     This subroutine is identical to DLAQR3 except that it avoids */
/*     recursion by calling DLAHQR instead of DLAQR4. */


/*     ****************************************************************** */
/*     Aggressive early deflation: */

/*     This subroutine accepts as input an upper Hessenberg matrix */
/*     H and performs an orthogonal similarity transformation */
/*     designed to detect and deflate fully converged eigenvalues from */
/*     a trailing principal submatrix.  On output H has been over- */
/*     written by a new Hessenberg matrix that is a perturbation of */
/*     an orthogonal similarity transformation of H.  It is to be */
/*     hoped that the final version of H has many zero subdiagonal */
/*     entries. */

/*     ****************************************************************** */
/*     WANTT   (input) LOGICAL */
/*          If .TRUE., then the Hessenberg matrix H is fully updated */
/*          so that the quasi-triangular Schur factor may be */
/*          computed (in cooperation with the calling subroutine). */
/*          If .FALSE., then only enough of H is updated to preserve */
/*          the eigenvalues. */

/*     WANTZ   (input) LOGICAL */
/*          If .TRUE., then the orthogonal matrix Z is updated so */
/*          so that the orthogonal Schur factor may be computed */
/*          (in cooperation with the calling subroutine). */
/*          If .FALSE., then Z is not referenced. */

/*     N       (input) INTEGER */
/*          The order of the matrix H and (if WANTZ is .TRUE.) the */
/*          order of the orthogonal matrix Z. */

/*     KTOP    (input) INTEGER */
/*          It is assumed that either KTOP = 1 or H(KTOP,KTOP-1)=0. */
/*          KBOT and KTOP together determine an isolated block */
/*          along the diagonal of the Hessenberg matrix. */

/*     KBOT    (input) INTEGER */
/*          It is assumed without a check that either */
/*          KBOT = N or H(KBOT+1,KBOT)=0.  KBOT and KTOP together */
/*          determine an isolated block along the diagonal of the */
/*          Hessenberg matrix. */

/*     NW      (input) INTEGER */
/*          Deflation window size.  1 .LE. NW .LE. (KBOT-KTOP+1). */

/*     H       (input/output) DOUBLE PRECISION array, dimension (LDH,N) */
/*          On input the initial N-by-N section of H stores the */
/*          Hessenberg matrix undergoing aggressive early deflation. */
/*          On output H has been transformed by an orthogonal */
/*          similarity transformation, perturbed, and the returned */
/*          to Hessenberg form that (it is to be hoped) has some */
/*          zero subdiagonal entries. */

/*     LDH     (input) int */
/*          Leading dimension of H just as declared in the calling */
/*          subroutine.  N .LE. LDH */

/*     ILOZ    (input) INTEGER */
/*     IHIZ    (input) INTEGER */
/*          Specify the rows of Z to which transformations must be */
/*          applied if WANTZ is .TRUE.. 1 .LE. ILOZ .LE. IHIZ .LE. N. */

/*     Z       (input/output) DOUBLE PRECISION array, dimension (LDZ,N) */
/*          IF WANTZ is .TRUE., then on output, the orthogonal */
/*          similarity transformation mentioned above has been */
/*          accumulated into Z(ILOZ:IHIZ,ILO:IHI) from the right. */
/*          If WANTZ is .FALSE., then Z is unreferenced. */

/*     LDZ     (input) int */
/*          The leading dimension of Z just as declared in the */
/*          calling subroutine.  1 .LE. LDZ. */

/*     NS      (output) int */
/*          The number of unconverged (ie approximate) eigenvalues */
/*          returned in SR and SI that may be used as shifts by the */
/*          calling subroutine. */

/*     ND      (output) int */
/*          The number of converged eigenvalues uncovered by this */
/*          subroutine. */

/*     SR      (output) DOUBLE PRECISION array, dimension KBOT */
/*     SI      (output) DOUBLE PRECISION array, dimension KBOT */
/*          On output, the float and imaginary parts of approximate */
/*          eigenvalues that may be used for shifts are stored in */
/*          SR(KBOT-ND-NS+1) through SR(KBOT-ND) and */
/*          SI(KBOT-ND-NS+1) through SI(KBOT-ND), respectively. */
/*          The float and imaginary parts of converged eigenvalues */
/*          are stored in SR(KBOT-ND+1) through SR(KBOT) and */
/*          SI(KBOT-ND+1) through SI(KBOT), respectively. */

/*     V       (workspace) DOUBLE PRECISION array, dimension (LDV,NW) */
/*          An NW-by-NW work array. */

/*     LDV     (input) int scalar */
/*          The leading dimension of V just as declared in the */
/*          calling subroutine.  NW .LE. LDV */

/*     NH      (input) int scalar */
/*          The number of columns of T.  NH.GE.NW. */

/*     T       (workspace) DOUBLE PRECISION array, dimension (LDT,NW) */

/*     LDT     (input) int */
/*          The leading dimension of T just as declared in the */
/*          calling subroutine.  NW .LE. LDT */

/*     NV      (input) int */
/*          The number of rows of work array WV available for */
/*          workspace.  NV.GE.NW. */

/*     WV      (workspace) DOUBLE PRECISION array, dimension (LDWV,NW) */

/*     LDWV    (input) int */
/*          The leading dimension of W just as declared in the */
/*          calling subroutine.  NW .LE. LDV */

/*     WORK    (workspace) DOUBLE PRECISION array, dimension LWORK. */
/*          On exit, WORK(1) is set to an estimate of the optimal value */
/*          of LWORK for the given values of N, NW, KTOP and KBOT. */

/*     LWORK   (input) int */
/*          The dimension of the work array WORK.  LWORK = 2*NW */
/*          suffices, but greater efficiency may result from larger */
/*          values of LWORK. */

/*          If LWORK = -1, then a workspace query is assumed; DLAQR2 */
/*          only estimates the optimal workspace size for the given */
/*          values of N, NW, KTOP and KBOT.  The estimate is returned */
/*          in WORK(1).  No error message related to LWORK is issued */
/*          by XERBLA.  Neither H nor Z are accessed. */

/*     ================================================================ */
/*     Based on contributions by */
/*        Karen Braman and Ralph Byers, Department of Mathematics, */
/*        University of Kansas, USA */

/*     ================================================================ */
/*     .. Parameters .. */
/*     .. */
/*     .. Local Scalars .. */
/*     .. */
/*     .. External Functions .. */
/*     .. */
/*     .. External Subroutines .. */
/*     .. */
/*     .. Intrinsic Functions .. */
/*     .. */
/*     .. Executable Statements .. */

/*     ==== Estimate optimal workspace. ==== */

    /* Parameter adjustments */
    h_dim1 = *ldh;
    h_offset = 1 + h_dim1;
    h__ -= h_offset;
    z_dim1 = *ldz;
    z_offset = 1 + z_dim1;
    z__ -= z_offset;
    --sr;
    --si;
    v_dim1 = *ldv;
    v_offset = 1 + v_dim1;
    v -= v_offset;
    t_dim1 = *ldt;
    t_offset = 1 + t_dim1;
    t -= t_offset;
    wv_dim1 = *ldwv;
    wv_offset = 1 + wv_dim1;
    wv -= wv_offset;
    --work;

    /* Function Body */
/* Computing MIN */
    i__1 = *nw, i__2 = *kbot - *ktop + 1;
    jw = MIN(i__1,i__2);
    if (jw <= 2) {
	lwkopt = 1;
    } else {

/*        ==== Workspace query call to DGEHRD ==== */

	i__1 = jw - 1;
	dgehrd_(&jw, &c__1, &i__1, &t[t_offset], ldt, &work[1], &work[1], &
		c_n1, &info);
	lwk1 = (int) work[1];

/*        ==== Workspace query call to DORMHR ==== */

	i__1 = jw - 1;
	dormhr_("R", "N", &jw, &jw, &c__1, &i__1, &t[t_offset], ldt, &work[1], 
		 &v[v_offset], ldv, &work[1], &c_n1, &info);
	lwk2 = (int) work[1];

/*        ==== Optimal workspace ==== */

	lwkopt = jw + MAX(lwk1,lwk2);
    }

/*     ==== Quick return in case of workspace query. ==== */

    if (*lwork == -1) {
	work[1] = (double) lwkopt;
	return 0;
    }

/*     ==== Nothing to do ... */
/*     ... for an empty active block ... ==== */
    *ns = 0;
    *nd = 0;
    work[1] = 1.;
    if (*ktop > *kbot) {
	return 0;
    }
/*     ... nor for an empty deflation window. ==== */
    if (*nw < 1) {
	return 0;
    }

/*     ==== Machine constants ==== */

    safmin = dlamch_("SAFE MINIMUM");
    safmax = 1. / safmin;
    dlabad_(&safmin, &safmax);
    ulp = dlamch_("PRECISION");
    smlnum = safmin * ((double) (*n) / ulp);

/*     ==== Setup deflation window ==== */

/* Computing MIN */
    i__1 = *nw, i__2 = *kbot - *ktop + 1;
    jw = MIN(i__1,i__2);
    kwtop = *kbot - jw + 1;
    if (kwtop == *ktop) {
	s = 0.;
    } else {
	s = h__[kwtop + (kwtop - 1) * h_dim1];
    }

    if (*kbot == kwtop) {

/*        ==== 1-by-1 deflation window: not much to do ==== */

	sr[kwtop] = h__[kwtop + kwtop * h_dim1];
	si[kwtop] = 0.;
	*ns = 1;
	*nd = 0;
/* Computing MAX */
	d__2 = smlnum, d__3 = ulp * (d__1 = h__[kwtop + kwtop * h_dim1], ABS(
		d__1));
	if (ABS(s) <= MAX(d__2,d__3)) {
	    *ns = 0;
	    *nd = 1;
	    if (kwtop > *ktop) {
		h__[kwtop + (kwtop - 1) * h_dim1] = 0.;
	    }
	}
	work[1] = 1.;
	return 0;
    }

/*     ==== Convert to spike-triangular form.  (In case of a */
/*     .    rare QR failure, this routine continues to do */
/*     .    aggressive early deflation using that part of */
/*     .    the deflation window that converged using INFQR */
/*     .    here and there to keep track.) ==== */

    dlacpy_("U", &jw, &jw, &h__[kwtop + kwtop * h_dim1], ldh, &t[t_offset], 
	    ldt);
    i__1 = jw - 1;
    i__2 = *ldh + 1;
    i__3 = *ldt + 1;
    dcopy_(&i__1, &h__[kwtop + 1 + kwtop * h_dim1], &i__2, &t[t_dim1 + 2], &
	    i__3);

    dlaset_("A", &jw, &jw, &c_b12, &c_b13, &v[v_offset], ldv);
    dlahqr_(&c_true, &c_true, &jw, &c__1, &jw, &t[t_offset], ldt, &sr[kwtop], 
	    &si[kwtop], &c__1, &jw, &v[v_offset], ldv, &infqr);

/*     ==== DTREXC needs a clean margin near the diagonal ==== */

    i__1 = jw - 3;
    for (j = 1; j <= i__1; ++j) {
	t[j + 2 + j * t_dim1] = 0.;
	t[j + 3 + j * t_dim1] = 0.;
/* L10: */
    }
    if (jw > 2) {
	t[jw + (jw - 2) * t_dim1] = 0.;
    }

/*     ==== Deflation detection loop ==== */

    *ns = jw;
    ilst = infqr + 1;
L20:
    if (ilst <= *ns) {
	if (*ns == 1) {
	    bulge = FALSE;
	} else {
	    bulge = t[*ns + (*ns - 1) * t_dim1] != 0.;
	}

/*        ==== Small spike tip test for deflation ==== */

	if (! bulge) {

/*           ==== Real eigenvalue ==== */

	    foo = (d__1 = t[*ns + *ns * t_dim1], ABS(d__1));
	    if (foo == 0.) {
		foo = ABS(s);
	    }
/* Computing MAX */
	    d__2 = smlnum, d__3 = ulp * foo;
	    if ((d__1 = s * v[*ns * v_dim1 + 1], ABS(d__1)) <= MAX(d__2,d__3))
		     {

/*              ==== Deflatable ==== */

		--(*ns);
	    } else {

/*              ==== Undeflatable.   Move it up out of the way. */
/*              .    (DTREXC can not fail in this case.) ==== */

		ifst = *ns;
		dtrexc_("V", &jw, &t[t_offset], ldt, &v[v_offset], ldv, &ifst, 
			 &ilst, &work[1], &info);
		++ilst;
	    }
	} else {

/*           ==== Complex conjugate pair ==== */

	    foo = (d__3 = t[*ns + *ns * t_dim1], ABS(d__3)) + sqrt((d__1 = t[*
		    ns + (*ns - 1) * t_dim1], ABS(d__1))) * sqrt((d__2 = t[*
		    ns - 1 + *ns * t_dim1], ABS(d__2)));
	    if (foo == 0.) {
		foo = ABS(s);
	    }
/* Computing MAX */
	    d__3 = (d__1 = s * v[*ns * v_dim1 + 1], ABS(d__1)), d__4 = (d__2 =
		     s * v[(*ns - 1) * v_dim1 + 1], ABS(d__2));
/* Computing MAX */
	    d__5 = smlnum, d__6 = ulp * foo;
	    if (MAX(d__3,d__4) <= MAX(d__5,d__6)) {

/*              ==== Deflatable ==== */

		*ns += -2;
	    } else {

/*              ==== Undeflatable. Move them up out of the way. */
/*              .    Fortunately, DTREXC does the right thing with */
/*              .    ILST in case of a rare exchange failure. ==== */

		ifst = *ns;
		dtrexc_("V", &jw, &t[t_offset], ldt, &v[v_offset], ldv, &ifst, 
			 &ilst, &work[1], &info);
		ilst += 2;
	    }
	}

/*        ==== End deflation detection loop ==== */

	goto L20;
    }

/*        ==== Return to Hessenberg form ==== */

    if (*ns == 0) {
	s = 0.;
    }

    if (*ns < jw) {

/*        ==== sorting diagonal blocks of T improves accuracy for */
/*        .    graded matrices.  Bubble sort deals well with */
/*        .    exchange failures. ==== */

	sorted = FALSE;
	i__ = *ns + 1;
L30:
	if (sorted) {
	    goto L50;
	}
	sorted = TRUE;

	kend = i__ - 1;
	i__ = infqr + 1;
	if (i__ == *ns) {
	    k = i__ + 1;
	} else if (t[i__ + 1 + i__ * t_dim1] == 0.) {
	    k = i__ + 1;
	} else {
	    k = i__ + 2;
	}
L40:
	if (k <= kend) {
	    if (k == i__ + 1) {
		evi = (d__1 = t[i__ + i__ * t_dim1], ABS(d__1));
	    } else {
		evi = (d__3 = t[i__ + i__ * t_dim1], ABS(d__3)) + sqrt((d__1 =
			 t[i__ + 1 + i__ * t_dim1], ABS(d__1))) * sqrt((d__2 =
			 t[i__ + (i__ + 1) * t_dim1], ABS(d__2)));
	    }

	    if (k == kend) {
		evk = (d__1 = t[k + k * t_dim1], ABS(d__1));
	    } else if (t[k + 1 + k * t_dim1] == 0.) {
		evk = (d__1 = t[k + k * t_dim1], ABS(d__1));
	    } else {
		evk = (d__3 = t[k + k * t_dim1], ABS(d__3)) + sqrt((d__1 = t[
			k + 1 + k * t_dim1], ABS(d__1))) * sqrt((d__2 = t[k + 
			(k + 1) * t_dim1], ABS(d__2)));
	    }

	    if (evi >= evk) {
		i__ = k;
	    } else {
		sorted = FALSE;
		ifst = i__;
		ilst = k;
		dtrexc_("V", &jw, &t[t_offset], ldt, &v[v_offset], ldv, &ifst, 
			 &ilst, &work[1], &info);
		if (info == 0) {
		    i__ = ilst;
		} else {
		    i__ = k;
		}
	    }
	    if (i__ == kend) {
		k = i__ + 1;
	    } else if (t[i__ + 1 + i__ * t_dim1] == 0.) {
		k = i__ + 1;
	    } else {
		k = i__ + 2;
	    }
	    goto L40;
	}
	goto L30;
L50:
	;
    }

/*     ==== Restore shift/eigenvalue array from T ==== */

    i__ = jw;
L60:
    if (i__ >= infqr + 1) {
	if (i__ == infqr + 1) {
	    sr[kwtop + i__ - 1] = t[i__ + i__ * t_dim1];
	    si[kwtop + i__ - 1] = 0.;
	    --i__;
	} else if (t[i__ + (i__ - 1) * t_dim1] == 0.) {
	    sr[kwtop + i__ - 1] = t[i__ + i__ * t_dim1];
	    si[kwtop + i__ - 1] = 0.;
	    --i__;
	} else {
	    aa = t[i__ - 1 + (i__ - 1) * t_dim1];
	    cc = t[i__ + (i__ - 1) * t_dim1];
	    bb = t[i__ - 1 + i__ * t_dim1];
	    dd = t[i__ + i__ * t_dim1];
	    dlanv2_(&aa, &bb, &cc, &dd, &sr[kwtop + i__ - 2], &si[kwtop + i__ 
		    - 2], &sr[kwtop + i__ - 1], &si[kwtop + i__ - 1], &cs, &
		    sn);
	    i__ += -2;
	}
	goto L60;
    }

    if (*ns < jw || s == 0.) {
	if (*ns > 1 && s != 0.) {

/*           ==== Reflect spike back into lower triangle ==== */

	    dcopy_(ns, &v[v_offset], ldv, &work[1], &c__1);
	    beta = work[1];
	    dlarfg_(ns, &beta, &work[2], &c__1, &tau);
	    work[1] = 1.;

	    i__1 = jw - 2;
	    i__2 = jw - 2;
	    dlaset_("L", &i__1, &i__2, &c_b12, &c_b12, &t[t_dim1 + 3], ldt);

	    dlarf_("L", ns, &jw, &work[1], &c__1, &tau, &t[t_offset], ldt, &
		    work[jw + 1]);
	    dlarf_("R", ns, ns, &work[1], &c__1, &tau, &t[t_offset], ldt, &
		    work[jw + 1]);
	    dlarf_("R", &jw, ns, &work[1], &c__1, &tau, &v[v_offset], ldv, &
		    work[jw + 1]);

	    i__1 = *lwork - jw;
	    dgehrd_(&jw, &c__1, ns, &t[t_offset], ldt, &work[1], &work[jw + 1]
, &i__1, &info);
	}

/*        ==== Copy updated reduced window into place ==== */

	if (kwtop > 1) {
	    h__[kwtop + (kwtop - 1) * h_dim1] = s * v[v_dim1 + 1];
	}
	dlacpy_("U", &jw, &jw, &t[t_offset], ldt, &h__[kwtop + kwtop * h_dim1]
, ldh);
	i__1 = jw - 1;
	i__2 = *ldt + 1;
	i__3 = *ldh + 1;
	dcopy_(&i__1, &t[t_dim1 + 2], &i__2, &h__[kwtop + 1 + kwtop * h_dim1], 
		 &i__3);

/*        ==== Accumulate orthogonal matrix in order update */
/*        .    H and Z, if requested.  ==== */

	if (*ns > 1 && s != 0.) {
	    i__1 = *lwork - jw;
	    dormhr_("R", "N", &jw, ns, &c__1, ns, &t[t_offset], ldt, &work[1], 
		     &v[v_offset], ldv, &work[jw + 1], &i__1, &info);
	}

/*        ==== Update vertical slab in H ==== */

	if (*wantt) {
	    ltop = 1;
	} else {
	    ltop = *ktop;
	}
	i__1 = kwtop - 1;
	i__2 = *nv;
	for (krow = ltop; i__2 < 0 ? krow >= i__1 : krow <= i__1; krow += 
		i__2) {
/* Computing MIN */
	    i__3 = *nv, i__4 = kwtop - krow;
	    kln = MIN(i__3,i__4);
	    dgemm_("N", "N", &kln, &jw, &jw, &c_b13, &h__[krow + kwtop * 
		    h_dim1], ldh, &v[v_offset], ldv, &c_b12, &wv[wv_offset], 
		    ldwv);
	    dlacpy_("A", &kln, &jw, &wv[wv_offset], ldwv, &h__[krow + kwtop * 
		    h_dim1], ldh);
/* L70: */
	}

/*        ==== Update horizontal slab in H ==== */

	if (*wantt) {
	    i__2 = *n;
	    i__1 = *nh;
	    for (kcol = *kbot + 1; i__1 < 0 ? kcol >= i__2 : kcol <= i__2; 
		    kcol += i__1) {
/* Computing MIN */
		i__3 = *nh, i__4 = *n - kcol + 1;
		kln = MIN(i__3,i__4);
		dgemm_("C", "N", &jw, &kln, &jw, &c_b13, &v[v_offset], ldv, &
			h__[kwtop + kcol * h_dim1], ldh, &c_b12, &t[t_offset], 
			 ldt);
		dlacpy_("A", &jw, &kln, &t[t_offset], ldt, &h__[kwtop + kcol *
			 h_dim1], ldh);
/* L80: */
	    }
	}

/*        ==== Update vertical slab in Z ==== */

	if (*wantz) {
	    i__1 = *ihiz;
	    i__2 = *nv;
	    for (krow = *iloz; i__2 < 0 ? krow >= i__1 : krow <= i__1; krow +=
		     i__2) {
/* Computing MIN */
		i__3 = *nv, i__4 = *ihiz - krow + 1;
		kln = MIN(i__3,i__4);
		dgemm_("N", "N", &kln, &jw, &jw, &c_b13, &z__[krow + kwtop * 
			z_dim1], ldz, &v[v_offset], ldv, &c_b12, &wv[
			wv_offset], ldwv);
		dlacpy_("A", &kln, &jw, &wv[wv_offset], ldwv, &z__[krow + 
			kwtop * z_dim1], ldz);
/* L90: */
	    }
	}
    }

/*     ==== Return the number of deflations ... ==== */

    *nd = jw - *ns;

/*     ==== ... and the number of shifts. (Subtracting */
/*     .    INFQR from the spike length takes care */
/*     .    of the case of a rare QR failure while */
/*     .    calculating eigenvalues of the deflation */
/*     .    window.)  ==== */

    *ns -= infqr;

/*      ==== Return optimal workspace. ==== */

    work[1] = (double) lwkopt;

/*     ==== End of DLAQR2 ==== */

    return 0;
} /* dlaqr2_ */
