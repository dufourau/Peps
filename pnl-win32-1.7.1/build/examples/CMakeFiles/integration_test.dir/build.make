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
CMAKE_SOURCE_DIR = /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build

# Include any dependencies generated for this target.
include examples/CMakeFiles/integration_test.dir/depend.make

# Include the progress variables for this target.
include examples/CMakeFiles/integration_test.dir/progress.make

# Include the compile flags for this target's objects.
include examples/CMakeFiles/integration_test.dir/flags.make

examples/CMakeFiles/integration_test.dir/integration_test.c.obj: examples/CMakeFiles/integration_test.dir/flags.make
examples/CMakeFiles/integration_test.dir/integration_test.c.obj: examples/CMakeFiles/integration_test.dir/includes_C.rsp
examples/CMakeFiles/integration_test.dir/integration_test.c.obj: ../examples/integration_test.c
	$(CMAKE_COMMAND) -E cmake_progress_report /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles $(CMAKE_PROGRESS_1)
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Building C object examples/CMakeFiles/integration_test.dir/integration_test.c.obj"
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && /usr/local/mingw32/bin/cc  $(C_DEFINES) $(C_FLAGS) -o CMakeFiles/integration_test.dir/integration_test.c.obj   -c /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/examples/integration_test.c

examples/CMakeFiles/integration_test.dir/integration_test.c.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing C source to CMakeFiles/integration_test.dir/integration_test.c.i"
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && /usr/local/mingw32/bin/cc  $(C_DEFINES) $(C_FLAGS) -E /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/examples/integration_test.c > CMakeFiles/integration_test.dir/integration_test.c.i

examples/CMakeFiles/integration_test.dir/integration_test.c.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling C source to assembly CMakeFiles/integration_test.dir/integration_test.c.s"
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && /usr/local/mingw32/bin/cc  $(C_DEFINES) $(C_FLAGS) -S /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/examples/integration_test.c -o CMakeFiles/integration_test.dir/integration_test.c.s

examples/CMakeFiles/integration_test.dir/integration_test.c.obj.requires:
.PHONY : examples/CMakeFiles/integration_test.dir/integration_test.c.obj.requires

examples/CMakeFiles/integration_test.dir/integration_test.c.obj.provides: examples/CMakeFiles/integration_test.dir/integration_test.c.obj.requires
	$(MAKE) -f examples/CMakeFiles/integration_test.dir/build.make examples/CMakeFiles/integration_test.dir/integration_test.c.obj.provides.build
.PHONY : examples/CMakeFiles/integration_test.dir/integration_test.c.obj.provides

examples/CMakeFiles/integration_test.dir/integration_test.c.obj.provides.build: examples/CMakeFiles/integration_test.dir/integration_test.c.obj

examples/CMakeFiles/integration_test.dir/tests_utils.c.obj: examples/CMakeFiles/integration_test.dir/flags.make
examples/CMakeFiles/integration_test.dir/tests_utils.c.obj: examples/CMakeFiles/integration_test.dir/includes_C.rsp
examples/CMakeFiles/integration_test.dir/tests_utils.c.obj: ../examples/tests_utils.c
	$(CMAKE_COMMAND) -E cmake_progress_report /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/CMakeFiles $(CMAKE_PROGRESS_2)
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Building C object examples/CMakeFiles/integration_test.dir/tests_utils.c.obj"
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && /usr/local/mingw32/bin/cc  $(C_DEFINES) $(C_FLAGS) -o CMakeFiles/integration_test.dir/tests_utils.c.obj   -c /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/examples/tests_utils.c

examples/CMakeFiles/integration_test.dir/tests_utils.c.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing C source to CMakeFiles/integration_test.dir/tests_utils.c.i"
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && /usr/local/mingw32/bin/cc  $(C_DEFINES) $(C_FLAGS) -E /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/examples/tests_utils.c > CMakeFiles/integration_test.dir/tests_utils.c.i

examples/CMakeFiles/integration_test.dir/tests_utils.c.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling C source to assembly CMakeFiles/integration_test.dir/tests_utils.c.s"
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && /usr/local/mingw32/bin/cc  $(C_DEFINES) $(C_FLAGS) -S /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/examples/tests_utils.c -o CMakeFiles/integration_test.dir/tests_utils.c.s

examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.requires:
.PHONY : examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.requires

examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.provides: examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.requires
	$(MAKE) -f examples/CMakeFiles/integration_test.dir/build.make examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.provides.build
.PHONY : examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.provides

examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.provides.build: examples/CMakeFiles/integration_test.dir/tests_utils.c.obj

# Object files for target integration_test
integration_test_OBJECTS = \
"CMakeFiles/integration_test.dir/integration_test.c.obj" \
"CMakeFiles/integration_test.dir/tests_utils.c.obj"

# External object files for target integration_test
integration_test_EXTERNAL_OBJECTS =

examples/integration_test.exe: examples/CMakeFiles/integration_test.dir/integration_test.c.obj
examples/integration_test.exe: examples/CMakeFiles/integration_test.dir/tests_utils.c.obj
examples/integration_test.exe: examples/CMakeFiles/integration_test.dir/build.make
examples/integration_test.exe: src/libpnl.dll.a
examples/integration_test.exe: /usr/local/mingw32/i686-pc-mingw32/bin/libblas.dll
examples/integration_test.exe: /usr/local/mingw32/i686-pc-mingw32/bin/liblapack.dll
examples/integration_test.exe: /usr/local/mingw32/i686-pc-mingw32/bin/libblas.dll
examples/integration_test.exe: /usr/local/mingw32/i686-pc-mingw32/bin/liblapack.dll
examples/integration_test.exe: examples/CMakeFiles/integration_test.dir/objects1.rsp
examples/integration_test.exe: examples/CMakeFiles/integration_test.dir/link.txt
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --red --bold "Linking C executable integration_test.exe"
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && $(CMAKE_COMMAND) -E cmake_link_script CMakeFiles/integration_test.dir/link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
examples/CMakeFiles/integration_test.dir/build: examples/integration_test.exe
.PHONY : examples/CMakeFiles/integration_test.dir/build

examples/CMakeFiles/integration_test.dir/requires: examples/CMakeFiles/integration_test.dir/integration_test.c.obj.requires
examples/CMakeFiles/integration_test.dir/requires: examples/CMakeFiles/integration_test.dir/tests_utils.c.obj.requires
.PHONY : examples/CMakeFiles/integration_test.dir/requires

examples/CMakeFiles/integration_test.dir/clean:
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples && $(CMAKE_COMMAND) -P CMakeFiles/integration_test.dir/cmake_clean.cmake
.PHONY : examples/CMakeFiles/integration_test.dir/clean

examples/CMakeFiles/integration_test.dir/depend:
	cd /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1 /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/examples /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/examples/CMakeFiles/integration_test.dir/DependInfo.cmake --color=$(COLOR)
.PHONY : examples/CMakeFiles/integration_test.dir/depend
