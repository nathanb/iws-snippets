#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
PREFIX=/opt/mono-master

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH


echo "updating existing system"
sudo apt-get update
sudo apt-get upgrade -y

echo "installing prerequisites"
sudo apt-get install -y build-essential libc6-dev g++ gcc libglib2.0-dev pkg-config git-core apache2 apache2-threaded-dev bison gettext autoconf automake libtool libpango1.0-dev libatk1.0-dev libgtk2.0-dev libtiff4-dev libgif-dev libglade2-dev git libgtk3.0-dev

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

PROJECTS=("mono"
"libgdiplus"
"gtk-sharp"
"xsp"
"mod_mono")

SOURCES=("git://github.com/mono/mono.git"
"git://github.com/mono/libgdiplus.git"
"git://github.com/mono/gtk-sharp.git"
"git://github.com/mono/xsp.git"
"git://github.com/mono/mod_mono.git")


cd $BUILDDIR

echo Cloning projects
for i in "${SOURCES[@]}"
do
git clone $i
done

echo
echo "building mono packages"
echo

for i in "${PROJECTS[@]}"
do
	cd $BUILDDIR/$i
	./autogen.sh --prefix=$PREFIX
	make
	
	if [ "$i" = ${PROJECTS[0]} ]
	then
		sudo make install
	fi
done

echo
echo "installing mono packages"
echo

for i in "${PROJECTS[@]:1}"
do
	cd $BUILDDIR/$i
	sudo make install
done

cd $BUILDDIR
echo
echo "done"


