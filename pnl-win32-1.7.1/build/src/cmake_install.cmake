# Install script for directory: /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/src

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
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE FILE FILES "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/libpnl.def")
endif()

if(NOT CMAKE_INSTALL_COMPONENT OR "${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified")
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE STATIC_LIBRARY OPTIONAL FILES "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/libpnl.dll.a")
endif()

if(NOT CMAKE_INSTALL_COMPONENT OR "${CMAKE_INSTALL_COMPONENT}" STREQUAL "Unspecified")
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/lib" TYPE SHARED_LIBRARY FILES "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/libpnl.dll")
  if(EXISTS "$ENV{DESTDIR}${CMAKE_INSTALL_PREFIX}/lib/libpnl.dll" AND
     NOT IS_SYMLINK "$ENV{DESTDIR}${CMAKE_INSTALL_PREFIX}/lib/libpnl.dll")
    if(CMAKE_INSTALL_DO_STRIP)
      execute_process(COMMAND "/usr/local/mingw32/i686-pc-mingw32/bin/strip" "$ENV{DESTDIR}${CMAKE_INSTALL_PREFIX}/lib/libpnl.dll")
    endif()
  endif()
endif()

if(NOT CMAKE_INSTALL_LOCAL_ONLY)
  # Include the install script for each subdirectory.
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/include/pnl/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/objects/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/libamos/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/librand/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/fft/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/math/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/linalg/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/sort/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/specfun/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/libcephes/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/libinteg/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/interpol/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/roots/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/libminpack/cmake_install.cmake")
  include("/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/src/optim/cmake_install.cmake")

endif()

