#!/usr/bin/perl -w
# compile-fortunes.pl by anotak
# this is for use with Doom Builder X in order to compile a fortunes raw text file into a .fortune file that dbx will load. 
# provided so people can compile their own fortunes files if they wish

# the reason for using this code is that it's faster to load, as the loading time will only required the time for reading 3 numbers and then the fortune itself as opposed to trying to parse a large text file every time you load dbx

# this code is distributed under the MIT License:

# Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
# The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

use warnings;
use strict;

open(INFILE,"fortune.txt") or die "unable to open fortune.txt: $! \n";

print("opened fortune.txt\n");

# set the "line separator"
$/ = "%%";

my @in_str = ();
my @in_len = ();

while(<INFILE>)
{
    #clean up extra whitespace chars
    chomp;
    s/^\s*//;
    s/\s*$//;
    
    
    my $len = length();
    # no empty strings
    if($len > 0)
    {
        push @in_str, $_;
        push @in_len, $len;
    }
}
close(INFILE);

open(OUTFILE, ">fortune.fortune");

print("writing " . scalar @in_str . " strings to the file");
# write num strings to to file
print(OUTFILE  pack("L",scalar @in_str ) );
binmode OUTFILE;

my $i = 0;
# size of header is 4 + 4 * size of array + 4 * size of array
# 4 for num elements
# per element
#   4 for ptr to element
#   4 for sizeof element
my $cur_ptr = 4 + (8 * scalar @in_str);
my $combined = "";

# write the headers :>
foreach(@in_str)
{
    #PRINT FOR DEBUGGING
    print "\n----------- len " . $in_len[$i] . " , string " . $i . " at ptr " . $cur_ptr . "\n" . $_ . "\n";
    
    my $pack_len = pack("L<", $in_len[$i]);
    
    print(OUTFILE $pack_len);
    
    my $pack_ptr = pack("L<", $cur_ptr);
    
    print(OUTFILE $pack_ptr);
    
    $cur_ptr += $in_len[$i];
    
    # save the actual string for after we write the header
    $combined .= $_;
    
    $i++;
}

# write all the strings
print(OUTFILE $combined);

close(OUTFILE);






#