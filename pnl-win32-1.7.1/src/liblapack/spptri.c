/* spptri.f -- translated by f2c (version 20061008).
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

static float c_b8 = 1.f;
static int c__1 = 1;

 int spptri_(char *uplo, int *n, float *ap, int *info)
{
    /* System generated locals */
    int i__1, i__2;

    /* Local variables */
    int j, jc, jj;
    float ajj;
    int jjn;
    extern double sdot_(int *, float *, int *, float *, int *);
    extern  int sspr_(char *, int *, float *, float *, 
	    int *, float *);
    extern int lsame_(char *, char *);
    extern  int sscal_(int *, float *, float *, int *);
    int upper;
    extern  int stpmv_(char *, char *, char *, int *, 
	    float *, float *, int *), xerbla_(char *
, int *), stptri_(char *, char *, int *, float *, 
	    int *);


/*  -- LAPACK routine (version 3.2) -- */
/*     Univ. of Tennessee, Univ. of California Berkeley and NAG Ltd.. */
/*     November 2006 */

/*     .. Scalar Arguments .. */
/*     .. */
/*     .. Array Arguments .. */
/*     .. */

/*  Purpose */
/*  ======= */

/*  SPPTRI computes the inverse of a float symmetric positive definite */
/*  matrix A using the Cholesky factorization A = U**T*U or A = L*L**T */
/*  computed by SPPTRF. */

/*  Arguments */
/*  ========= */

/*  UPLO    (input) CHARACTER*1 */
/*          = 'U':  Upper triangular factor is stored in AP; */
/*          = 'L':  Lower triangular factor is stored in AP. */

/*  N       (input) INTEGER */
/*          The order of the matrix A.  N >= 0. */

/*  AP      (input/output) REAL array, dimension (N*(N+1)/2) */
/*          On entry, the triangular factor U or L from the Cholesky */
/*          factorization A = U**T*U or A = L*L**T, packed columnwise as */
/*          a linear array.  The j-th column of U or L is stored in the */
/*          array AP as follows: */
/*          if UPLO = 'U', AP(i + (j-1)*j/2) = U(i,j) for 1<=i<=j; */
/*          if UPLO = 'L', AP(i + (j-1)*(2n-j)/2) = L(i,j) for j<=i<=n. */

/*          On exit, the upper or lower triangle of the (symmetric) */
/*          inverse of A, overwriting the input factor U or L. */

/*  INFO    (output) INTEGER */
/*          = 0:  successful exit */
/*          < 0:  if INFO = -i, the i-th argument had an illegal value */
/*          > 0:  if INFO = i, the (i,i) element of the factor U or L is */
/*                zero, and the inverse could not be computed. */

/*  ===================================================================== */

/*     .. Parameters .. */
/*     .. */
/*     .. Local Scalars .. */
/*     .. */
/*     .. External Functions .. */
/*     .. */
/*     .. External Subroutines .. */
/*     .. */
/*     .. Executable Statements .. */

/*     Test the input parameters. */

    /* Parameter adjustments */
    --ap;

    /* Function Body */
    *info = 0;
    upper = lsame_(uplo, "U");
    if (! upper && ! lsame_(uplo, "L")) {
	*info = -1;
    } else if (*n < 0) {
	*info = -2;
    }
    if (*info != 0) {
	i__1 = -(*info);
	xerbla_("SPPTRI", &i__1);
	return 0;
    }

/*     Quick return if possible */

    if (*n == 0) {
	return 0;
    }

/*     Invert the triangular Cholesky factor U or L. */

    stptri_(uplo, "Non-unit", n, &ap[1], info);
    if (*info > 0) {
	return 0;
    }

    if (upper) {

/*        Compute the product inv(U) * inv(U)'. */

	jj = 0;
	i__1 = *n;
	for (j = 1; j <= i__1; ++j) {
	    jc = jj + 1;
	    jj += j;
	    if (j > 1) {
		i__2 = j - 1;
		sspr_("Upper", &i__2, &c_b8, &ap[jc], &c__1, &ap[1]);
	    }
	    ajj = ap[jj];
	    sscal_(&j, &ajj, &ap[jc], &c__1);
/* L10: */
	}

    } else {

/*        Compute the product inv(L)' * inv(L). */

	jj = 1;
	i__1 = *n;
	for (j = 1; j <= i__1; ++j) {
	    jjn = jj + *n - j + 1;
	    i__2 = *n - j + 1;
	    ap[jj] = sdot_(&i__2, &ap[jj], &c__1, &ap[jj], &c__1);
	    if (j < *n) {
		i__2 = *n - j;
		stpmv_("Lower", "Transpose", "Non-unit", &i__2, &ap[jjn], &ap[
			jj + 1], &c__1);
	    }
	    jj = jjn;
/* L20: */
	}
    }

    return 0;

/*     End of SPPTRI */

} /* spptri_ */