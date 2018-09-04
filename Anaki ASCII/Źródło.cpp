#include <stdio.h>
#include <stdlib.h>

int main(void)
{
	int a = 0;
	for ( a; a < 256; a = a + 3)
	{
		printf("\t%c     %d \t %c     %d \t %c     %d \n", a, a, a + 1, a + 1, a + 2, a + 2);
	}
	system("pause");
}