#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build-tools
PREFIX=/opt/mono-2.10

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

declare -A PACKAGEFILES
PACKAGEFILES=(["mono-addins-0.5"]="http://ftp.novell.com/pub/mono/sources/mono-addins/mono-addins-0.5.tar.bz2"
["mono-debugger-2.10"]="http://ftp.novell.com/pub/mono/sources/mono-debugger/mono-debugger-2.10.tar.bz2"
["mono-tools-2.10"]="http://ftp.novell.com/pub/mono/sources/mono-tools/mono-tools-2.10.tar.bz2"
["gnome-sharp-2.24.1"]="http://ftp.novell.com/pub/mono/sources/gnome-sharp2/gnome-sharp-2.24.1.tar.bz2"
["monodevelop-2.4.2"]="http://ftp.novell.com/pub/mono/sources/monodevelop/monodevelop-2.4.2.tar.bz2"
["monodevelop-debugger-mdb-2.4"]="http://ftp.novell.com/pub/mono/sources/monodevelop-debugger-mdb/monodevelop-debugger-mdb-2.4.tar.bz2"
["monodevelop-debugger-gdb-2.4"]="http://ftp.novell.com/pub/mono/sources/monodevelop-debugger-gdb/monodevelop-debugger-gdb-2.4.tar.bz2"
["monodevelop-database-2.4"]="http://ftp.novell.com/pub/mono/sources/monodevelop-database/monodevelop-database-2.4.tar.bz2"
)

echo Downloading
for i in "${!PACKAGEFILES[@]}"
do
	#only download it if you don't already have it. 
	if [ ! -f "$i.tar" ]
	then
		wget ${PACKAGEFILES[$i]}
	fi
	if [ -f "$i.tar.bz2" ]
	then
		bunzip2 -df "$i.tar.bz2"
	fi
	if [ -f "$i.tar" ]
	then
		tar -xvf "$i.tar"
	fi
done


echo
echo "building and installing mono packages"
echo

for i in "${!PACKAGEFILES[@]}"
do
	cd $BUILDDIR/$i
	./configure --prefix=$PREFIX
	make
	sudo make install
done

cd $BUILDDIR
echo
echo "done"
