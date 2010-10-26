#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
DLDDIR=$TOPDIR/downloads

cd $BUILDDIR

echo "building and installing mono packages"
echo

cd $BUILDDIR
cd libgdiplus-*
sudo make uninstall

cd $BUILDDIR
cd gtk-sharp-*
sudo make uninstall

cd $BUILDDIR
cd mod_mono-*
sudo make uninstall

cd $BUILDDIR
cd xsp-*
sudo make uninstall

cd $BUILDDIR
cd mono-*
sudo make uninstall

cd $BUILDDIR
rm -rf mono-*

echo
echo "done"