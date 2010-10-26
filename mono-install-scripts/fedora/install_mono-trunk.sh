#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
DLDDIR=$TOPDIR/downloads

export PATH=/opt/mono-daily/bin:$PATH

echo "updating existing system"
yum update -y

echo "installing prerequisites"
yum install -y make automake glibc-devel gcc-c++ gcc glib2-devel pkgconfig subversion bison gettext lib autoconf httpd httpd-devel libtool wget

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

svn co svn://anonsvn.mono-project.com/source/trunk/xsp
svn co svn://anonsvn.mono-project.com/source/trunk/mod_mono

wget http://mono.ximian.com/daily/mono-latest.tar.bz2

cd $BUILDDIR
bunzip2 -df mono-latest.tar.bz2
tar -xvf mono-latest.tar

echo
echo "building and installing mono packages"
echo
cd $BUILDDIR
cd mono-*
./configure --prefix=/opt/mono-daily
make
make install

cd $BUILDDIR
cd xsp
./autogen.sh --prefix=/opt/mono-daily
make
make install

cd $BUILDDIR
cd mod_mono
./autogen.sh --prefix=/opt/mono-daily
make
make install
cd $BUILDDIR

echo
echo "done"
