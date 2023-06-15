# ramsey
Code for finding and examining extremal cases for the Ramsey numbers

This is C# code for a Bachelor's thesis on the Ramsey numbers, using statistical physics and network theory. The basis for finding extremal cases of the Ramsey numbers is
a Metropolis algorithm, where the Hamiltonian is given by the number of monochromatic cliques of the size of interest.
From the Program class, one can run simulations for finding extremal cases, while the other commented functions are for examining extremal cases. Simply
uncomment a function and set the parameters as you wish. Some functions export data to a text file, you will have to change the paths or disable the
export. One may use simple Python plotting applications to create histograms or graphs (e.g. matplotlib).
