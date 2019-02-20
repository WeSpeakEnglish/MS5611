#ifndef MS5611_H
#define MS5611_H

#include <stdint.h>

void MS5611_init(uint8_t SensNumb);  // init the sensor

uint16_t MS5611_cmd_prom(uint8_t SensNumb,uint8_t coef_num); //take coeffs from the sensor
uint8_t MS5611_crc4(uint16_t n_prom[]);
void MS5611_Calculate(double * T, double * P, uint16_t * C, uint32_t D1, uint32_t D2);
void MS5611_SS_Set(uint8_t SensNumb, uint8_t LogLev); // Chip Select
uint32_t MS5611_cmd_adc(uint8_t SensNumb, uint8_t cmd);

extern uint8_t SPI2_Buffer_Rx[32];


#define MS5611_CS1_PIN GPIO_PIN_12
#define MS5611_CS1_PORT GPIOB
#define MS5611_CS2_PIN GPIO_PIN_10
#define MS5611_CS2_PORT GPIOB

#define CMD_RESET 0x1E // ADC reset command
#define CMD_ADC_READ 0x00 // ADC read command
#define CMD_ADC_CONV 0x40 // ADC conversion command
#define CMD_ADC_D1 0x00 // ADC D1 conversion
#define CMD_ADC_D2 0x10 // ADC D2 conversion
#define CMD_ADC_256 0x00 // ADC OSR=256
#define CMD_ADC_512 0x02 // ADC OSR=512
#define CMD_ADC_1024 0x04 // ADC OSR=1024
#define CMD_ADC_2048 0x06 // ADC OSR=2056
#define CMD_ADC_4096 0x08 // ADC OSR=4096
#define CMD_PROM_RD 0xA0 // Prom read command 
#define PROM_NB                 8

#endif