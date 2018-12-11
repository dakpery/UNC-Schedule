from datetime import *
from bitstring import BitArray, BitStream
import io
import re
import mysql.connector
from mysql.connector import errorcode

# Dictionary used by get_index
days = {
    'm': 0,
    't': 1,
    'w': 2,
    'r': 3,
    'f' : 4,
    's' : 5,
    'u' : 6
}


def time_to_military(time):
    # Takes an am/pm time string and returns a time string in military format
    time = datetime.strptime(time, '%I:%M %p')
    time = datetime.strftime(time, '%H:%M')
    return time


def get_index (time, day):
    """
    Returns indexes for the bitarray of size 168
    Assumes 7am-7pm, forces a floor or ceiling of 7am/7pm
    Assumes 30 minute bins, starting at :00 and :30
    """
    time = time.lstrip('0').split(':')
    if int(time[0]) < 7:
        time[0] = 7
    elif int(time[0]) > 19:
        time[0] = 19
    index = int(time[0]) * 2 - 14 + (24 * int(day)) # Formula
    if int(time[1]) > 29:
        index += 1
    return index


def format_strings(TIME, DAYS):
    """
    Helper function for get_bit_array, converts day and time strings to D*D*D T*T*T, ex:
    days = 'M*W'
    times = '02:30 pm-04:20 pm*02:30 pm-04:20 pm'
    """
    temp_times = TIME.split('*')
    day_time = ''
    i = 0
    DAYS = DAYS.replace('SU', 'U')
    for char in DAYS:
        if char != '*':
            day_time += char + temp_times[i] + '*'
        else:
            i += 1

    day_time = day_time.rstrip('*').split("*")
    TIME = ''
    DAYS = ''
    for string in day_time:
        try:
            DAYS += string[0] + '*'
            TIME += string[1:] + '*'
        except Exception as e:
            print(e)
            return
    TIME = TIME.rstrip('*')
    DAYS = DAYS.rstrip('*').lower()
    return TIME, DAYS


def set_array(bit_array, start_index, end_index):
    for x in range(start_index, end_index + 1):
        bit_array[x] = 1


def get_bit_array(day_string, time_string):
    formatted_time, formatted_days = format_strings(time_string, day_string)
    day = formatted_days.split('*')
    time = formatted_time.split('*')
    i = 0
    while i < len(time):
        if time[i] == 'TBA':
            del time[i]
            del day[i]
        else:
            i += 1
    for x in range(0,len(time)):
        time[x] = time[x].split('-')

    for x in range(0,len(time)):
        for y in range(0,len(time[x])):
            time[x][y] = time_to_military(time[x][y])

    indices = []
    for x in range(0, len(time)):
        indices.append(get_index(time[x][0], days[day[x]]))
        indices.append(get_index(time[x][1], days[day[x]]))

    a = BitArray(168)

    for x in range(0, len(indices), 2):
        set_array(a, indices[x], indices[x+1])

    return a

# Dictionary for local mysql database connection, make sure 'password' is correct
config = {
    'user': 'root',
    'password': 'password',
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

cursor = cnx.cursor(buffered=True)
query = ("SELECT CRN, DAYS, TIME, COURSENUM, LOCATION, BITARRAY FROM course_table") # table name = 'course_table'
cursor.execute(query)
result = cursor.fetchall()  # copy the data and iterate through the copy rather than cursor,
                            # otherwise cursor.execute() will break the upcoming loop after 1 iteration

ignored = 0
updated = 0
for (CRN, DAYS, TIME, COURSENUM, LOCATION, BITARRAY) in result:
    if COURSENUM > 499 or LOCATION.startswith('ONL') or DAYS == ' ' or DAYS == 'TBA' or TIME == 'TBA':
        # Ignore Online and Grad classes, as well as classes without specified days or times
        ignored += 1
    else:
        # print('{},{},{}'.format(CRN, DAYS, TIME))
        bitarray = get_bit_array(DAYS, TIME)
        update = """UPDATE course_table SET BITARRAY = %s WHERE CRN = %s"""
        data = (str(bitarray.bin), CRN)
        cursor.execute(update, data)
        updated += 1

cnx.commit() # Important!
cursor.close() # Close everything
cnx.close()
print('Ignored {} Online/Grad classes'.format(ignored))
print('Added {} bitarrays to the database'.format(updated))


# for reading from a text file: #
# file = open('courses', 'r')
# filtered = filter(lambda x: not re.match(r'^\s*$', x), file)
# list = []
# for line in filtered:
#     list.append(line.strip('\n').split(','))
# file.close()

# print(list)

# Outputs strings you can paste into workbench #
# bitarrays = []
# for item in list:
#     if item[1] != 'TBA' and item[1] != '':
#         try:
#             bitarrays.append([item[0], get_bit_array(item[2].lower(), item[1])])
#         except Exception as e:
#             print(e, item)
#
# for array in bitarrays:
#      print("UPDATE course_table SET BITARRAY = " + "'" + array[1].bin + "'" + " WHERE CRN = " + array[0] + ";")

