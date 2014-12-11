/* sorg2l.f -- translated by f2c (version 20061008).
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

 int sorg2l_(int *m, int *n, int *k, float *a, 
	int *lda, float *tau, float *work, int *info)
{
    /* System generated locals */
    int a_dim1, a_offset, i__1, i__2, i__3;
    float r__1;

    /* Local variables */
    int i__, j, l, ii;
    extern  int sscal_(int *, float *, float *, int *), 
	    slarf_(char *, int *, int *, float *, int *, float *, 
	    float *, int *, float *), xerbla_(char *, int *);


/*  -- LAPACK routine (version 3.2) -- */
/*     Univ. of Tennessee, Univ. of California Berkeley and NAG Ltd.. */
/*     November 2006 */

/*     .. Scalar Arguments .. */
/*     .. */
/*     .. Array Arguments .. */
/*     .. */

/*  Purpose */
/*  ======= */

/*  SORG2L generates an m by n float matrix Q with orthonormal columns, */
/*  which is defined as the last n columns of a product of k elementary */
/*  reflectors of order m */

/*        Q  =  H(k) . . . H(2) H(1) */

/*  as returned by SGEQLF. */

/*  Arguments */
/*  ========= */

/*  M       (input) INTEGER */
/*          The number of rows of the matrix Q. M >= 0. */

/*  N       (input) INTEGER */
/*          The number of columns of the matrix Q. M >= N >= 0. */

/*  K       (input) INTEGER */
/*          The number of elementary reflectors whose product defines the */
/*          matrix Q. N >= K >= 0. */

/*  A       (input/output) REAL array, dimension (LDA,N) */
/*          On entry, the (n-k+i)-th column must contain the vector which */
/*          defines the elementary reflector H(i), for i = 1,2,...,k, as */
/*          returned by SGEQLF in the last k columns of its array */
/*          argument A. */
/*          On exit, the m by n matrix Q. */

/*  LDA     (input) INTEGER */
/*          The first dimension of the array A. LDA >= MAX(1,M). */

/*  TAU     (input) REAL array, dimension (K) */
/*          TAU(i) must contain the scalar factor of the elementary */
/*          reflector H(i), as returned by SGEQLF. */

/*  WORK    (workspace) REAL array, dimension (N) */

/*  INFO    (output) INTEGER */
/*          = 0: successful exit */
/*          < 0: if INFO = -i, the i-th argument has an illegal value */

/*  ===================================================================== */

/*     .. Parameters .. */
/*     .. */
/*     .. Local Scalars .. */
/*     .. */
/*     .. External Subroutines .. */
/*     .. */
/*     .. Intrinsic Functions .. */
/*     .. */
/*     .. Executable Statements .. */

/*     Test the input arguments */

    /* Parameter adjustments */
    a_dim1 = *lda;
    a_offset = 1 + a_dim1;
    a -= a_offset;
    --tau;
    --work;

    /* Function Body */
    *info = 0;
    if (*m < 0) {
	*info = -1;
    } else if (*n < 0 || *n > *m) {
	*info = -2;
    } else if (*k < 0 || *k > *n) {
	*info = -3;
    } else if (*lda < MAX(1,*m)) {
	*info = -5;
    }
    if (*info != 0) {
	i__1 = -(*info);
	xerbla_("SORG2L", &i__1);
	return 0;
    }

/*     Quick return if possible */

    if (*n <= 0) {
	return 0;
    }

/*     Initialise columns 1:n-k to columns of the unit matrix */

    i__1 = *n - *k;
    for (j = 1; j <= i__1; ++j) {
	i__2 = *m;
	for (l = 1; l <= i__2; ++l) {
	    a[l + j * a_dim1] = 0.f;
/* L10: */
	}
	a[*m - *n + j + j * a_dim1] = 1.f;
/* L20: */
    }

    i__1 = *k;
    for (i__ = 1; i__ <= i__1; ++i__) {
	ii = *n - *k + i__;

/*        Apply H(i) to A(1:m-k+i,1:n-k+i) from the left */

	a[*m - *n + ii + ii * a_dim1] = 1.f;
	i__2 = *m - *n + ii;
	i__3 = ii - 1;
	slarf_("Left", &i__2, &i__3, &a[ii * a_dim1 + 1], &c__1, &tau[i__], &
		a[a_offset], lda, &work[1]);
	i__2 = *m - *n + ii - 1;
	r__1 = -tau[i__];
	sscal_(&i__2, &r__1, &a[ii * a_dim1 + 1], &c__1);
	a[*m - *n + ii + ii * a_dim1] = 1.f - tau[i__];

/*        Set A(m-k+i+1:m,n-k+i) to zero */

	i__2 = *m;
	for (l = *m - *n + ii + 1; l <= i__2; ++l) {
	    a[l + ii * a_dim1] = 0.f;
/* L30: */
	}
/* L40: */
    }
    return 0;

/*     End of SORG2L */

} /* sorg2l_ */
