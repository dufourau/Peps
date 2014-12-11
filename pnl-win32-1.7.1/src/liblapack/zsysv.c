/* zsysv.f -- translated by f2c (version 20061008).
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

 int zsysv_(char *uplo, int *n, int *nrhs, 
	doublecomplex *a, int *lda, int *ipiv, doublecomplex *b, 
	int *ldb, doublecomplex *work, int *lwork, int *info)
{
    /* System generated locals */
    int a_dim1, a_offset, b_dim1, b_offset, i__1;

    /* Local variables */
    int nb;
    extern int lsame_(char *, char *);
    extern  int xerbla_(char *, int *);
    extern int ilaenv_(int *, char *, char *, int *, int *, 
	    int *, int *);
    int lwkopt;
    int lquery;
    extern  int zsytrf_(char *, int *, doublecomplex *, 
	    int *, int *, doublecomplex *, int *, int *), zsytrs_(char *, int *, int *, doublecomplex *, 
	    int *, int *, doublecomplex *, int *, int *);


/*  -- LAPACK driver routine (version 3.2) -- */
/*     Univ. of Tennessee, Univ. of California Berkeley and NAG Ltd.. */
/*     November 2006 */

/*     .. Scalar Arguments .. */
/*     .. */
/*     .. Array Arguments .. */
/*     .. */

/*  Purpose */
/*  ======= */

/*  ZSYSV computes the solution to a complex system of linear equations */
/*     A * X = B, */
/*  where A is an N-by-N symmetric matrix and X and B are N-by-NRHS */
/*  matrices. */

/*  The diagonal pivoting method is used to factor A as */
/*     A = U * D * U**T,  if UPLO = 'U', or */
/*     A = L * D * L**T,  if UPLO = 'L', */
/*  where U (or L) is a product of permutation and unit upper (lower) */
/*  triangular matrices, and D is symmetric and block diagonal with */
/*  1-by-1 and 2-by-2 diagonal blocks.  The factored form of A is then */
/*  used to solve the system of equations A * X = B. */

/*  Arguments */
/*  ========= */

/*  UPLO    (input) CHARACTER*1 */
/*          = 'U':  Upper triangle of A is stored; */
/*          = 'L':  Lower triangle of A is stored. */

/*  N       (input) INTEGER */
/*          The number of linear equations, i.e., the order of the */
/*          matrix A.  N >= 0. */

/*  NRHS    (input) INTEGER */
/*          The number of right hand sides, i.e., the number of columns */
/*          of the matrix B.  NRHS >= 0. */

/*  A       (input/output) COMPLEX*16 array, dimension (LDA,N) */
/*          On entry, the symmetric matrix A.  If UPLO = 'U', the leading */
/*          N-by-N upper triangular part of A contains the upper */
/*          triangular part of the matrix A, and the strictly lower */
/*          triangular part of A is not referenced.  If UPLO = 'L', the */
/*          leading N-by-N lower triangular part of A contains the lower */
/*          triangular part of the matrix A, and the strictly upper */
/*          triangular part of A is not referenced. */

/*          On exit, if INFO = 0, the block diagonal matrix D and the */
/*          multipliers used to obtain the factor U or L from the */
/*          factorization A = U*D*U**T or A = L*D*L**T as computed by */
/*          ZSYTRF. */

/*  LDA     (input) INTEGER */
/*          The leading dimension of the array A.  LDA >= MAX(1,N). */

/*  IPIV    (output) INTEGER array, dimension (N) */
/*          Details of the interchanges and the block structure of D, as */
/*          determined by ZSYTRF.  If IPIV(k) > 0, then rows and columns */
/*          k and IPIV(k) were interchanged, and D(k,k) is a 1-by-1 */
/*          diagonal block.  If UPLO = 'U' and IPIV(k) = IPIV(k-1) < 0, */
/*          then rows and columns k-1 and -IPIV(k) were interchanged and */
/*          D(k-1:k,k-1:k) is a 2-by-2 diagonal block.  If UPLO = 'L' and */
/*          IPIV(k) = IPIV(k+1) < 0, then rows and columns k+1 and */
/*          -IPIV(k) were interchanged and D(k:k+1,k:k+1) is a 2-by-2 */
/*          diagonal block. */

/*  B       (input/output) COMPLEX*16 array, dimension (LDB,NRHS) */
/*          On entry, the N-by-NRHS right hand side matrix B. */
/*          On exit, if INFO = 0, the N-by-NRHS solution matrix X. */

/*  LDB     (input) INTEGER */
/*          The leading dimension of the array B.  LDB >= MAX(1,N). */

/*  WORK    (workspace/output) COMPLEX*16 array, dimension (MAX(1,LWORK)) */
/*          On exit, if INFO = 0, WORK(1) returns the optimal LWORK. */

/*  LWORK   (input) INTEGER */
/*          The length of WORK.  LWORK >= 1, and for best performance */
/*          LWORK >= MAX(1,N*NB), where NB is the optimal blocksize for */
/*          ZSYTRF. */

/*          If LWORK = -1, then a workspace query is assumed; the routine */
/*          only calculates the optimal size of the WORK array, returns */
/*          this value as the first entry of the WORK array, and no error */
/*          message related to LWORK is issued by XERBLA. */

/*  INFO    (output) INTEGER */
/*          = 0: successful exit */
/*          < 0: if INFO = -i, the i-th argument had an illegal value */
/*          > 0: if INFO = i, D(i,i) is exactly zero.  The factorization */
/*               has been completed, but the block diagonal matrix D is */
/*               exactly singular, so the solution could not be computed. */

/*  ===================================================================== */

/*     .. Local Scalars .. */
/*     .. */
/*     .. External Functions .. */
/*     .. */
/*     .. External Subroutines .. */
/*     .. */
/*     .. Intrinsic Functions .. */
/*     .. */
/*     .. Executable Statements .. */

/*     Test the input parameters. */

    /* Parameter adjustments */
    a_dim1 = *lda;
    a_offset = 1 + a_dim1;
    a -= a_offset;
    --ipiv;
    b_dim1 = *ldb;
    b_offset = 1 + b_dim1;
    b -= b_offset;
    --work;

    /* Function Body */
    *info = 0;
    lquery = *lwork == -1;
    if (! lsame_(uplo, "U") && ! lsame_(uplo, "L")) {
	*info = -1;
    } else if (*n < 0) {
	*info = -2;
    } else if (*nrhs < 0) {
	*info = -3;
    } else if (*lda < MAX(1,*n)) {
	*info = -5;
    } else if (*ldb < MAX(1,*n)) {
	*info = -8;
    } else if (*lwork < 1 && ! lquery) {
	*info = -10;
    }

    if (*info == 0) {
	if (*n == 0) {
	    lwkopt = 1;
	} else {
	    nb = ilaenv_(&c__1, "ZSYTRF", uplo, n, &c_n1, &c_n1, &c_n1);
	    lwkopt = *n * nb;
	}
	work[1].r = (double) lwkopt, work[1].i = 0.;
    }

    if (*info != 0) {
	i__1 = -(*info);
	xerbla_("ZSYSV ", &i__1);
	return 0;
    } else if (lquery) {
	return 0;
    }

/*     Compute the factorization A = U*D*U' or A = L*D*L'. */

    zsytrf_(uplo, n, &a[a_offset], lda, &ipiv[1], &work[1], lwork, info);
    if (*info == 0) {

/*        Solve the system A*X = B, overwriting B with X. */

	zsytrs_(uplo, n, nrhs, &a[a_offset], lda, &ipiv[1], &b[b_offset], ldb, 
		 info);

    }

    work[1].r = (double) lwkopt, work[1].i = 0.;

    return 0;

/*     End of ZSYSV */

} /* zsysv_ */
