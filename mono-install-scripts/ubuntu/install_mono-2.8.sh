#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
PREFIX=/opt/mono-2.8.2

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH


echo "updating existing system"
sudo apt-get update
sudo apt-get upgrade -y

echo "installing prerequisites"
sudo apt-get install -y build-essential libc6-dev g++ gcc libglib2.0-dev pkg-config git-core apache2 apache2-threaded-dev bison gettext autoconf automake libtool libpango1.0-dev libatk1.0-dev libgtk2.0-dev libtiff4-dev libgif-dev libglade2-dev

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

declare -A PACKAGEFILES
PACKAGEFILES=(["libgdiplus-2.8.1"]="http://ftp.novell.com/pub/mono/sources/libgdiplus/libgdiplus-2.8.1.tar.bz2"
["mono-2.8.2"]="http://ftp.novell.com/pub/mono/sources/mono/mono-2.8.2.tar.bz2"
["gtk-sharp-2.12.10"]="http://ftp.novell.com/pub/mono/sources/gtk-sharp212/gtk-sharp-2.12.10.tar.bz2"
["xsp-2.8.2"]="http://ftp.novell.com/pub/mono/sources/xsp/xsp-2.8.2.tar.bz2"
["mod_mono-2.8"]="http://ftp.novell.com/pub/mono/sources/mod_mono/mod_mono-2.8.tar.bz2")

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
