#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build

export PATH=/opt/mono-2.10/bin:$PATH
export PKG_CONFIG_PATH=/opt/mono-2.10/lib/pkgconfig:$PKG_CONFIG_PATH

cd $BUILDDIR

wget http://ftp.novell.com/pub/mono/sources/mono/mono-2.10.1.tar.bz2

cd $BUILDDIR
bunzip2 -df mono-2.10.1.tar.bz2
tar -xvf mono-2.10.1.tar

echo
echo "building and installing mono packages"
echo
cd $BUILDDIR
cd mono-2.10
sudo make uninstall

cd $BUILDDIR
cd mono-2.10.1
./configure --prefix=/opt/mono-2.10
make
sudo make install

cd $BUILDDIR

echo
echo "done"
