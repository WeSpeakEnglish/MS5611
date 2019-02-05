#ifndef MS5611_H
#define MS5611_H

#include <stdint.h>

void MS5611_init(void);
uint16_t ms5611_prom(uint8_t coef_num);
uint16_t MS5611_cmd_prom(uint8_t coef_num);
void ms5611_calculate(int32_t *pressure, int32_t *temperature);
uint8_t MS5611_crc4(uint16_t n_prom[]);
uint32_t MS5611_cmd_adc(uint8_t cmd);

extern uint8_t SPI2_Buffer_Rx[32];


#define MS5611_CS_PIN GPIO_PIN_12
#define MS5611_CS_PORT GPIOB

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