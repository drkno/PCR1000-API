PCR1000-API
===========

Icom PCR1000 Radio Receiver API written in C#. Supports network streaming.

Takes significant inspiration from the c++ icomlib.

Was designed for use in Cerberus (https://github.com/mrkno/Cerberus). I currently use Cerberus to control my PCR1000 radio so that WXtoImg can produce weather sat pictures: http://sat.makereti.co.nz/

NOTE: this is incompatible with Mono as it uses the DataReceivedEvent from the Mono SerialPort class implementation (which is far from complete and compatible).
