#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
DLDDIR=$TOPDIR/downloads

export PATH=/opt/mono-daily/bin:$PATH

cd $BUILDDIR

echo "building and installing mono packages"
echo
cd $BUILDDIR

cd mod_mono
sudo make uninstall

cd $BUILDDIR
cd xsp
sudo make uninstall

cd $BUILDDIR
cd mono-*
sudo make uninstall

cd $BUILDDIR
rm -rf mono-*

wget http://mono.ximian.com/daily/mono-latest.tar.bz2

bunzip2 -df mono-latest.tar.bz2
tar -xvf mono-latest.tar

cd $BUILDDIR
cd mono-*
./configure --prefix=/opt/mono-daily
make
sudo make install

cd $BUILDDIR
cd xsp-*
./configure.sh --prefix=/opt/mono-daily
make
sudo make install

cd $BUILDDIR
cd mod_mono-*
./configure.sh --prefix=/opt/mono-daily
make
sudo make install
cd $BUILDDIR


echo
echo "done"