# CMAKE generated file: DO NOT EDIT!
# Generated by "Unix Makefiles" Generator, CMake Version 3.0

#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:

# Remove some rules from gmake that .SUFFIXES does not remove.
SUFFIXES =

.SUFFIXES: .hpux_make_needs_suffix_list

# Suppress display of executed commands.
$(VERBOSE).SILENT:

# A target that is always out of date.
cmake_force:
.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

# The shell in which to execute make rules.
SHELL = /bin/sh

# The CMake executable.
CMAKE_COMMAND = /usr/local/Cellar/cmake/3.0.0/bin/cmake

# The command to remove a file.
RM = /usr/local/Cellar/cmake/3.0.0/bin/cmake -E remove -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface

# Include any dependencies generated for this target.
include CMakeFiles/FortranCInterface.dir/depend.make

# Include the progress variables for this target.
include CMakeFiles/FortranCInterface.dir/progress.make

# Include the compile flags for this target's objects.
include CMakeFiles/FortranCInterface.dir/flags.make

CMakeFiles/FortranCInterface.dir/main.F.obj: CMakeFiles/FortranCInterface.dir/flags.make
CMakeFiles/FortranCInterface.dir/main.F.obj: /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface/main.F
	$(CMAKE_COMMAND) -E cmake_progress_report /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface/CMakeFiles $(CMAKE_PROGRESS_1)
	@echo "Building Fortran object CMakeFiles/FortranCInterface.dir/main.F.obj"
	/usr/local/mingw32/bin/gfortran  $(Fortran_DEFINES) -DCALL_MOD $(Fortran_FLAGS) -c /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface/main.F -o CMakeFiles/FortranCInterface.dir/main.F.obj

CMakeFiles/FortranCInterface.dir/main.F.obj.requires:
.PHONY : CMakeFiles/FortranCInterface.dir/main.F.obj.requires

CMakeFiles/FortranCInterface.dir/main.F.obj.provides: CMakeFiles/FortranCInterface.dir/main.F.obj.requires
	$(MAKE) -f CMakeFiles/FortranCInterface.dir/build.make CMakeFiles/FortranCInterface.dir/main.F.obj.provides.build
.PHONY : CMakeFiles/FortranCInterface.dir/main.F.obj.provides

CMakeFiles/FortranCInterface.dir/main.F.obj.provides.build: CMakeFiles/FortranCInterface.dir/main.F.obj

CMakeFiles/FortranCInterface.dir/call_sub.f.obj: CMakeFiles/FortranCInterface.dir/flags.make
CMakeFiles/FortranCInterface.dir/call_sub.f.obj: /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface/call_sub.f
	$(CMAKE_COMMAND) -E cmake_progress_report /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface/CMakeFiles $(CMAKE_PROGRESS_2)
	@echo "Building Fortran object CMakeFiles/FortranCInterface.dir/call_sub.f.obj"
	/usr/local/mingw32/bin/gfortran  $(Fortran_DEFINES) $(Fortran_FLAGS) -c /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface/call_sub.f -o CMakeFiles/FortranCInterface.dir/call_sub.f.obj

CMakeFiles/FortranCInterface.dir/call_sub.f.obj.requires:
.PHONY : CMakeFiles/FortranCInterface.dir/call_sub.f.obj.requires

CMakeFiles/FortranCInterface.dir/call_sub.f.obj.provides: CMakeFiles/FortranCInterface.dir/call_sub.f.obj.requires
	$(MAKE) -f CMakeFiles/FortranCInterface.dir/build.make CMakeFiles/FortranCInterface.dir/call_sub.f.obj.provides.build
.PHONY : CMakeFiles/FortranCInterface.dir/call_sub.f.obj.provides

CMakeFiles/FortranCInterface.dir/call_sub.f.obj.provides.build: CMakeFiles/FortranCInterface.dir/call_sub.f.obj

CMakeFiles/FortranCInterface.dir/call_mod.f90.obj: CMakeFiles/FortranCInterface.dir/flags.make
CMakeFiles/FortranCInterface.dir/call_mod.f90.obj: /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface/call_mod.f90
	$(CMAKE_COMMAND) -E cmake_progress_report /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface/CMakeFiles $(CMAKE_PROGRESS_3)
	@echo "Building Fortran object CMakeFiles/FortranCInterface.dir/call_mod.f90.obj"
	/usr/local/mingw32/bin/gfortran  $(Fortran_DEFINES) $(Fortran_FLAGS) -c /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface/call_mod.f90 -o CMakeFiles/FortranCInterface.dir/call_mod.f90.obj

CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.requires:
.PHONY : CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.requires

CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.provides: CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.requires
	$(MAKE) -f CMakeFiles/FortranCInterface.dir/build.make CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.provides.build
.PHONY : CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.provides

CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.provides.build: CMakeFiles/FortranCInterface.dir/call_mod.f90.obj

# Object files for target FortranCInterface
FortranCInterface_OBJECTS = \
"CMakeFiles/FortranCInterface.dir/main.F.obj" \
"CMakeFiles/FortranCInterface.dir/call_sub.f.obj" \
"CMakeFiles/FortranCInterface.dir/call_mod.f90.obj"

# External object files for target FortranCInterface
FortranCInterface_EXTERNAL_OBJECTS =

FortranCInterface.exe: CMakeFiles/FortranCInterface.dir/main.F.obj
FortranCInterface.exe: CMakeFiles/FortranCInterface.dir/call_sub.f.obj
FortranCInterface.exe: CMakeFiles/FortranCInterface.dir/call_mod.f90.obj
FortranCInterface.exe: CMakeFiles/FortranCInterface.dir/build.make
FortranCInterface.exe: libsymbols.a
FortranCInterface.exe: libmyfort.a
FortranCInterface.exe: CMakeFiles/FortranCInterface.dir/objects1.rsp
FortranCInterface.exe: CMakeFiles/FortranCInterface.dir/link.txt
	@echo "Linking Fortran executable FortranCInterface.exe"
	$(CMAKE_COMMAND) -E cmake_link_script CMakeFiles/FortranCInterface.dir/link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
CMakeFiles/FortranCInterface.dir/build: FortranCInterface.exe
.PHONY : CMakeFiles/FortranCInterface.dir/build

CMakeFiles/FortranCInterface.dir/requires: CMakeFiles/FortranCInterface.dir/main.F.obj.requires
CMakeFiles/FortranCInterface.dir/requires: CMakeFiles/FortranCInterface.dir/call_sub.f.obj.requires
CMakeFiles/FortranCInterface.dir/requires: CMakeFiles/FortranCInterface.dir/call_mod.f90.obj.requires
.PHONY : CMakeFiles/FortranCInterface.dir/requires

CMakeFiles/FortranCInterface.dir/clean:
	$(CMAKE_COMMAND) -P CMakeFiles/FortranCInterface.dir/cmake_clean.cmake
.PHONY : CMakeFiles/FortranCInterface.dir/clean

CMakeFiles/FortranCInterface.dir/depend:
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface /usr/local/Cellar/cmake/3.0.0/share/cmake/Modules/FortranCInterface /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles/FortranCInterface/CMakeFiles/FortranCInterface.dir/DependInfo.cmake
.PHONY : CMakeFiles/FortranCInterface.dir/depend
