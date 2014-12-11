# Install script for directory: /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl

# Set the install prefix
if(NOT DEFINED CMAKE_INSTALL_PREFIX)
  set(CMAKE_INSTALL_PREFIX "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/../")
endif()
string(REGEX REPLACE "/$" "" CMAKE_INSTALL_PREFIX "${CMAKE_INSTALL_PREFIX}")

# Set the install configuration name.
if(NOT DEFINED CMAKE_INSTALL_CONFIG_NAME)
  if(BUILD_TYPE)
    string(REGEX REPLACE "^[^A-Za-z0-9_]+" ""
           CMAKE_INSTALL_CONFIG_NAME "${BUILD_TYPE}")
  else()
    set(CMAKE_INSTALL_CONFIG_NAME "Debug")
  endif()
  message(STATUS "Install configuration: \"${CMAKE_INSTALL_CONFIG_NAME}\"")
endif()

# Set the component getting installed.
if(NOT CMAKE_INSTALL_COMPONENT)
  if(COMPONENT)
    message(STATUS "Install component: \"${COMPONENT}\"")
    set(CMAKE_INSTALL_COMPONENT "${COMPONENT}")
  else()
    set(CMAKE_INSTALL_COMPONENT)
  endif()
endif()

if(NOT CMAKE_INSTALL_COMPONENT OR "${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified")
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/include/pnl" TYPE FILE FILES
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_array.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_band_matrix.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_basis.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_cdf.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_complex.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_deprecated.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_fft.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_finance.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_integration.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_interpolation.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_laplace.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_linalgsolver.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_list.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_mathtools.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_matrix.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_matvect.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_mpi.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_object.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_optim.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_perm.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_random.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_root.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_sp_matrix.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_specfun.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_templates_off.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_templates_on.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_tridiag_matrix.h"
    "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src/include/pnl/pnl_vector.h"
    )
endif()

