# - uninstall target template
#
if (NOT EXISTS "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/install_manifest.txt")
    message(FATAL_ERROR "Cannot find install manifest: /Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/install_manifest.txt")
endif(NOT EXISTS "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/install_manifest.txt")

file(READ "/Users/jl/tmp/pnl-20140722/pnl-win32-1.7.1/build/install_manifest.txt" files)
string(REGEX REPLACE "\n" ";" files "${files}")
string(REGEX REPLACE ";$" "" files "${files}")
list(REVERSE files)
foreach (file ${files})
    message(STATUS "Uninstalling $ENV{DESTDIR}${file}")
    if (EXISTS "$ENV{DESTDIR}${file}")
        execute_process(
            COMMAND /usr/local/Cellar/cmake/3.0.0/bin/cmake -E remove "$ENV{DESTDIR}${file}"
            OUTPUT_VARIABLE rm_out
            RESULT_VARIABLE rm_retval
        )
        if(NOT ${rm_retval} EQUAL 0)
            message(FATAL_ERROR "Problem when removing $ENV{DESTDIR}")
        endif (NOT ${rm_retval} EQUAL 0)
    else (EXISTS "$ENV{DESTDIR}")
        message(STATUS "File $ENV{DESTDIR}${file} does not exist.")
    endif (EXISTS "$ENV{DESTDIR}${file}")
endforeach(file)

