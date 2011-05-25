#!/bin/bash -e

TOPDIR=$(pwd)
BUILDDIR=$TOPDIR/build
PREFIX=/opt/mono-2.10

export PATH=$PREFIX/bin:$PATH
export PKG_CONFIG_PATH=$PREFIX/lib/pkgconfig:$PKG_CONFIG_PATH
export LD_LIBRARY_PATH=$PREFIX/lib:$LD_LIBRARY_PATH


echo "updating existing system"
sudo apt-get update
sudo apt-get upgrade -y

echo "installing prerequisites"
sudo apt-get install -y build-essential libc6-dev g++ gcc libglib2.0-dev pkg-config git-core apache2 apache2-threaded-dev bison gettext autoconf automake libtool libpango1.0-dev libatk1.0-dev libgtk2.0-dev libtiff4-dev libgif-dev libglade2-dev gnome-devel libgnomecanvas2-dev libgnomeui-dev xulrunner-1.9.2-dev

mkdir -p $BUILDDIR

echo
echo "downloading mono packages"
echo

cd $BUILDDIR

PACKAGES=("mono-addins-0.6"
"mono-debugger-2.10"
"mono-tools-2.10"
"gnome-sharp-2.24.1"
"monodevelop-2.5.92"
"monodevelop-debugger-gdb-2.5.92"
"monodevelop-database-2.5.92"
"gluezilla-2.6"
)

URLS=("http://ftp.novell.com/pub/mono/sources/mono-addins/mono-addins-0.6.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/mono-debugger/mono-debugger-2.10.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/mono-tools/mono-tools-2.10.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/gnome-sharp2/gnome-sharp-2.24.1.tar.bz2"
"http://monodevelop.com/files/Linux/tarballs/monodevelop-2.5.92.tar.bz2"
"http://monodevelop.com/files/Linux/tarballs/monodevelop-debugger-gdb-2.5.92.tar.bz2"
"http://monodevelop.com/files/Linux/tarballs/monodevelop-database-2.5.92.tar.bz2"
"http://ftp.novell.com/pub/mono/sources/gluezilla/gluezilla-2.6.tar.bz2"
)

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
echo "Building commenting tool for broken tests."
echo

COMMENTCS="$TOPDIR/comment-lines.cs"
COMMENTEXE="$TOPDIR/comment-lines.exe"

echo "using System; using System.Linq; using System.Collections.Generic; using System.Text; using System.IO; namespace FixGnomeSharp { public class Program { public static int Main(string[] args) { string fileName = null; string[] sLines = null; int[] lines = null; if (args != null) { foreach (var arg in args) { if (arg.StartsWith(\"-f:\") && arg.Length > 3) fileName = arg.Substring(3); if (arg.StartsWith(\"-l:\") && arg.Length > 3) { sLines = arg.Substring(3).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); lines = new int[sLines.Length]; for (int ix = 0; ix < sLines.Length; ix++) lines[ix] = int.Parse(sLines[ix]); } } } if (lines == null || lines.Length == 0 || fileName == null || !File.Exists(fileName)) { PrintUsage(); return 1; } /*comment line 221, 449 and 450*/ using (var ms = new MemoryStream()) { using (var writer = new StreamWriter(ms, new UTF8Encoding(false, false))) { using (var fileStream = File.OpenRead(fileName)) { using (var reader = new StreamReader(fileStream, new UTF8Encoding(false, false))) { int ix = 1; while (!reader.EndOfStream) { var line = reader.ReadLine(); if (lines.Contains(ix)) writer.Write('#'); writer.Write(line + '\n'); ix++; } } } } File.WriteAllBytes(fileName, ms.ToArray()); } return 0; } static void PrintUsage() { Console.WriteLine(\"Cannot comment without both arguments. Usage: comment-lines -f:FILE -l:LINE#[,LINE#...]\"); } } }" > "$COMMENTCS"

if [ -f "$COMMENTCS" ]
then
	gmcs -out:$COMMENTEXE $COMMENTCS
fi

echo
echo "building mono packages"
echo

for i in "${PACKAGES[@]}"
do
	cd $BUILDDIR/$i
	./configure --prefix=$PREFIX

#	need to comment out a few lines in gnome-sharp due to a test bug. 
	if [ $i == "gnome-sharp-2.24.1" ]
	then
		mono $COMMENTEXE -f:$BUILDDIR/$i/sample/gnomevfs/Makefile -l:221,449,450
	fi

	make
	sudo make install
done

cd $TOPDIR
rm comment-lines*

echo "creating a launcher in $TOPDIR"

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

echo
echo "done"


