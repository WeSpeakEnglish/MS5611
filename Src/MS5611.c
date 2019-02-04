#include "stm32f1xx_hal.h"
#include "dma.h"
#include "rtc.h"
#include "spi.h"
#include "usart.h"
#include "gpio.h"
#include "MS5611.h"


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

#define MS5611_CS_PIN 12
#define MS5611_CS_PORT GPIOB

uint8_t SPI2_Buffer_Rx[32];

void MS5611_init(void)
{
  uint8_t b;
HAL_GPIO_WritePin(MS5611_CS_PORT, MS5611_CS_PIN, GPIO_PIN_RESET);
	HAL_Delay(1);
        b = CMD_RESET;
	HAL_SPI_Transmit_DMA(&hspi2, &b, 1);
        HAL_SPI_Receive_DMA(&hspi2, SPI2_Buffer_Rx,0);
	HAL_Delay(1);
        HAL_GPIO_WritePin(MS5611_CS_PORT, MS5611_CS_PIN, GPIO_PIN_SET);
    return;
}