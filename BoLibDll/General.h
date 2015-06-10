#pragma once 

#define VC_EXTRALEAN

#include <Windows.h>
#include <bo.h>
#include <NVR.H>

#define MODEL_NUMBER 1527


// The Dave Matthews Band approve of this copy+paste from 1524/fmodel.h
#define UTIL_REQUEST_PRINT_TICKET		Share2&0x00000001
#define UTIL_REQUEST_EEPROM_UPDATE		Share2&0x00000002
#define UTIL_REQUEST_CHANGE_RNV			Share2&0x00000004
#define UTIL_REQUEST_EMPTY_RECYCLER		Share2&0x00000008
#define UTIL_REQUEST_RECYCLER_VALUE		Share2&0x00000010
#define UTIL_REQUEST_TITO_AUDIT			Share2&0x00000020
#define UTIL_REQUEST_ADD_2_CREDIT		Share2&0x00000040
#define UTIL_REQUEST_REFILL_COINS		Share2&0x00000080

