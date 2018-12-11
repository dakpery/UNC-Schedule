import random
import time
from bitstring import BitArray
import mysql.connector
from mysql.connector import errorcode



def generate_random(list):
    crns = []
    bitarrays = []
    for courselist in list:
        added = False
        i = 0
        while not added and i < len(courselist):
            if courselist[i][1] is None:
                crns.append(courselist[i][0])
                bitarrays.append(BitArray(168))
                added = True
            else:
                r = random.randint(0, len(courselist)-1)
                crns.append(courselist[r][0])
                if courselist[r][1] is None:
                    bitarrays.append(BitArray(168))
                else:
                    bitarrays.append(BitArray(bin=courselist[r][1]))
                added = True
    return crns, bitarrays


def score_schedule(bits):
    str = bits.bin
    gaps = 0
    for i in range(0, len(str)):
        if i % 24 == 0:
            counting = False
            last_1 = i
        if str[i] == '1':
            if i - last_1 > 1:
                if counting:
                    gaps += i - last_1 - 1
            counting = True
            last_1 = i
    return gaps


def check_conflicts_2(bitarray_list):
    bits_and = bitarray_list[0] | bitarray_list[1]
    bits_or = bitarray_list[0] | bitarray_list[1]
    for i in range(2,len(bitarray_list)):
        bits_and &= bitarray_list[i]
        if bool(bits_and):
            return True, bits_and
        else:

            bits_or |= bitarray_list[i]
            bits_and = BitArray(bin=bits_or.bin)

    return False, bits_or


config = {
    'user': 'root',
    'password': 'root',
    'host': '127.0.0.1',
    'database': 'courses',
    'raise_on_warnings': True
}

try:
    cnx = mysql.connector.connect(**config)
except mysql.connector.Error as err:
    if err.errno == errorcode.ER_ACCESS_DENIED_ERROR:
        print("Something is wrong with your user name or password")
    elif err.errno == errorcode.ER_BAD_DB_ERROR:
        print("Database does not exist")
    else:
        print(err)

desired_courses = [['SPN', 101], ['HST', 101], ['GER', 101], ['FRH', 101], ['MAT', 151]]
master_list = []
for item in desired_courses:
    cursor = cnx.cursor(buffered=True)
    query = """SELECT CRN, SUBJECT, COURSENUM, BITARRAY FROM course_table WHERE SUBJECT = %s and COURSENUM = %s""" # table name = 'course_table'
    data = (item[0], item[1])
    cursor.execute(query, data)
    result = cursor.fetchall()
    temp = []
    no_bitarray = []
    has_bitarray = []
    for (CRN, SUBJECT, COURSENUM, BITARRAY) in result:
        if BITARRAY == None:
            no_bitarray.append([CRN, BITARRAY])
        else:
            has_bitarray.append([CRN, BITARRAY])
        # print(CRN, SUBJECT, COURSENUM, BITARRAY)
    temp = no_bitarray + has_bitarray
    master_list.append(temp)
print(master_list)

cursor.close() # Close everything
cnx.close()


accepted = False
attempts = 0
start = time.clock()
while not accepted:
    attempts += 1
    crns, bitarrays = generate_random(master_list)
    conflict, bits = check_conflicts_2(bitarrays)
    if not conflict:
        score = score_schedule(bits)
        if score < 3:
            accepted = True
end = time.clock()

for i in range(len(crns)):
    print(crns[i], bitarrays[i].bin)
print('Score:', score, bits.bin)
print('Attempts:', attempts)
print('Calc time:', end - start, 'seconds')

# score = score_schedule(BitArray(bin='000000000011100000000000001111110000000000000000000000000011100000000000001111110000000000000000000000000011100000000000000000000000000000000000000000000000000000000000'))
# print('Score:', score)
