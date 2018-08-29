# ProSecCo

This repository contains the C# implementation of the ProSecCo algorithm for
interactive frequent sequence mining, as described in the paper:

[Sacha Servan-Schreiber](mailto:aservans@cs.brown.edu),
[Matteo Riondato](mailto:riondato@acm.org), [E. Zgraggen](mailto:emzg@mit.edu),
_ProSecCo: Progressive Sequence Mining with Convergence Guarantees_ appearing in
the Proceedings of the 18th IEEE International Conference on Data Mining
([ICDM'18](http://www.icdm2018.org)).

An extended version of this work is available in the
[prosecco.pdf](prosecco.pdf) file.

## Compilation and execution

To compile and run ProSecCo:
```sh
$ cd src/ProSecCo
$ dotnet restore
$ dotnet run -- --help
$ dotnet run -- --file <file-path> (--support <min-support> | --top-k <k>) \
	--size <block-size> --db-size <database size>
```

### PrefixSpan (sequence mining algorithm)

A C# implementation of the PrefixSpan algorithm for frequent sequence mining is
also included in this repository. This implementation is used by ProSecCo in the
sequence mining phase.

To compile and run PrefixSpan:
```sh
cd src/PrefixSpan
dotnet restore
dotnet run -- --help
dotnet run -- PrefixSpan --file <file-path> --support <min-support>
```

### File format

Both algorithms expect input files to be in
[SPMF](http://www.philippe-fournier-viger.com/spmf/) format.


## License

See the [LICENSE](./LICENSE) and the [NOTICE](./NOTICE) files for copyright and
license information.
