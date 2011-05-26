#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
PREFIX=/opt/mono-2.10

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH


echo "updating existing system"
yum update -y

echo "installing prerequisites"
yum install -y make automake glibc-devel gcc-c++ gcc glib2-devel pkgconfig subversion bison gettext-libs autoconf httpd httpd-devel libtool wget libtiff-devel libexif-devel libexif libjpeg-devel gtk2-devel atk-devel pango-devel giflib-devel libglade2-develui-dev libgnomeui-devel libgnome-devel xulrunner-devel libgnomecanvas2-devel

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

OLD=("monodevelop-2.5.91"
"monodevelop-debugger-gdb-2.5.91"
"monodevelop-database-2.5.91"
)


PACKAGES=("monodevelop-2.5.92"
"monodevelop-debugger-gdb-2.5.92"
"monodevelop-database-2.5.92"
)

URLS=("http://monodevelop.com/files/Linux/tarballs/monodevelop-2.5.92.tar.bz2"
"http://monodevelop.com/files/Linux/tarballs/monodevelop-debugger-gdb-2.5.92.tar.bz2"
"http://monodevelop.com/files/Linux/tarballs/monodevelop-database-2.5.92.tar.bz2"
)
for i in "${OLD[@]}"
do
	cd $BUILDDIR/$i
	sudo make uninstall
done

cd $BUILDDIR
echo Downloading
count=${#PACKAGES[@]}
index=0
while [ "$index" -lt "$count" ]
do
	#only download it if you don't already have it. 
	if [ ! -f "${PACKAGES[$index]}.tar" ]
	then
		wget "${URLS[@]:$index:1}"
	fi
	if [ -f "${PACKAGES[$index]}.tar.bz2" ]
	then
		bunzip2 -df "${PACKAGES[$index]}.tar.bz2"
	fi
	if [ -f "${PACKAGES[$index]}.tar" ]
	then
		tar -xvf "${PACKAGES[$index]}.tar"
	fi
	
	let "index = $index + 1"
done

echo
echo "building mono packages"
echo

for i in "${PACKAGES[@]}"
do
	cd $BUILDDIR/$i
	./configure --prefix=$PREFIX

	make
	sudo make install
done

cd $TOPDIR

echo "creating a launcher in $TOPDIR"

if [ ! -f "$TOPDIR/monodevelop-launcher.sh" ]
then
echo "#!/bin/bash
MONO_PREFIX=$PREFIX
GNOME_PREFIX=/usr
export DYLD_LIBRARY_FALLBACK_PATH=$MONO_PREFIX/lib:$DYLD_LIBRARY_FALLBACK_PATH
export LD_LIBRARY_PATH=$MONO_PREFIX/lib:$LD_LIBRARY_PATH
export C_INCLUDE_PATH=$MONO_PREFIX/include:$GNOME_PREFIX/include
export ACLOCAL_PATH=$MONO_PREFIX/share/aclocal
export PKG_CONFIG_PATH=$MONO_PREFIX/lib/pkgconfig:$GNOME_PREFIX/lib/pkgconfig
export PATH=$MONO_PREFIX/bin:$PATH

monodevelop" > "$TOPDIR/monodevelop-launcher.sh"

chmod 755 "$TOPDIR/monodevelop-launcher.sh"
fi

echo
echo "done"


