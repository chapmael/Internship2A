#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import zmq 
from CalculExtern import PreWorkAI


## Server ##
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

while True:
    #  Wait for next request from client
    message = socket.recv()


    #  print("Received request: %s" % message)
    print("Receive message:%s" % message)
    obj=PreWorkAI()
    ligne=obj.ReturnLastLine()
    #  Do some 'work' in the AI script
    #  Get information of the AI script 

    # Prepare information to send them ( pre calcul, type, ...)
    # Resend them 

    socket.send(str.encode(ligne))
    print("Answer sending ...")