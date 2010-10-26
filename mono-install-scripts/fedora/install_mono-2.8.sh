#!/bin/bash

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
DLDDIR=$TOPDIR/downloads

export PATH=/opt/mono-2.8/bin:$PATH
echo "#!/bin/bash
export PATH=$PATH" >> /etc/profile.d/mono-path.sh

echo "updating existing system"
yum update -y

echo "installing prerequisites"
yum install -y make automake glibc-devel gcc-c++ gcc glib2-devel pkgconfig subversion bison gettext lib autoconf httpd httpd-devel libtool wget libtiff-devel gtk2-devel atk-devel pango-devel giflib-devel libglade2-devel

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

wget http://ftp.novell.com/pub/mono/sources/xsp/xsp-2.8.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mod_mono/mod_mono-2.8.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/mono/mono-2.8.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/libgdiplus/libgdiplus-2.8.tar.bz2
wget http://ftp.novell.com/pub/mono/sources/gtk-sharp212/gtk-sharp-2.12.10.tar.bz2

cd $BUILDDIR
bunzip2 -df xsp-2.8.tar.bz2
tar -xvf xsp-2.8.tar

bunzip2 -df mod_mono-2.8.tar.bz2
tar -xvf mod_mono-2.8.tar

bunzip2 -df mono-2.8.tar.bz2
tar -xvf mono-2.8.tar

bunzip2 -df libgdiplus-2.8.tar.bz2
tar -xvf libgdiplus-2.8.tar

bunzip2 -df gtk-sharp-2.12.10.tar.bz2
tar -xvf gtk-sharp-2.12.10.tar

echo
echo "building and installing mono packages"
echo


cd $BUILDDIR
cd libgdiplus-2.8
./configure --prefix=/opt/mono-2.8
make
sudo make install

cd $BUILDDIR
cd mono-2.8
./configure --prefix=/opt/mono-2.8
make
sudo make install

cd $BUILDDIR
cd gtk-sharp-2.12.10
./configure --prefix=/opt/mono-2.8
make
sudo make install

cd $BUILDDIR
cd xsp-2.8
./configure --prefix=/opt/mono-2.8
make
sudo make install

cd $BUILDDIR
cd mod_mono-2.8
./configure --prefix=/opt/mono-2.8
make
sudo make install
cd $BUILDDIR

echo
echo "done"
