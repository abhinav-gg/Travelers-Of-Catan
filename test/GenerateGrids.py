
cache = []

for x in range(-2, 3):
    for y in range(-2, 3):
        for z in range(-2, 3):

            if x + y + z == 0:
                cache.append((x, y, z))

print(cache)
print(len(cache))

cache = []

for x in range(-2, 4):
    for y in range(-2, 4):
        for z in range(-2, 4):
            if x + y + z in [1, 2]:
                cache.append([x, y, z])


            
cache.sort()
print(cache)

print(len(cache))

inputs = """3, 0, -2

3, -2, 0

3, -1, -1

3, 0, -1

3, -1, 0

3, -2, 1

3, 1, -2

2, 1, -2

2, 0, -1

2, -1, 0

2, -2, 1

2, 2, -2

2, 1, -1

2, 0, 0

2, -1, 1

2, -2, 2

1, -2, 2

1, -1, 1

1, 2, -2

1, 1, -1

1, 0, 0

1, 3, -2

1, -1, 2

1, 2, -1

1, 1, 0

1, 0, 1

1, -2, 3

0, 3, -2

0, -2, 3

0, -1, 2

0, 2, -1

0, 1, 0

0, 0, 1

0, 2, 0

0, 0, 2

0, 3, -1

0, 1, 1

0, -1, 3

-1, 2, 0

-1, 0, 2

-1, -1, 3

-1, 1, 1

-1, 3, -1

-1, 2, 1

-1, 1, 2

-1, 3, 0

-1, 0, 3

-2, 0, 3

-2, 1, 2

-2, 3, 0

-2, 2, 1

-2, 1, 3

-2, 3, 1

-2, 2, 2
"""
p = inputs.split("\n\n")
print(len(p))
p = [list(map(int, y.split(","))) for y in p]
p.sort()
print(p)
print(p == cache)
