#include "stm32f1xx_hal.h"
#include "dma.h"
#include "rtc.h"
#include "spi.h"
#include "usart.h"
#include "gpio.h"
#include "MS5611.h"
#include "math.h"





#define SS1_SetLow   HAL_GPIO_WritePin(MS5611_CS_PORT, MS5611_CS_PIN, GPIO_PIN_RESET)
#define SS1_SetHigh  HAL_GPIO_WritePin(MS5611_CS_PORT, MS5611_CS_PIN, GPIO_PIN_SET)

#define spi_send(X) {b = X; HAL_SPI_Transmit(&hspi2, &b, 1, 1000);}


uint8_t SPI2_Buffer_Rx[32];

uint32_t ms5611_ut;  // static result of temperature measurement
uint32_t ms5611_up;  // static result of pressure measurement
uint16_t ms5611_c[PROM_NB];  // on-chip ROM
static uint8_t ms5611_osr = CMD_ADC_4096;
uint8_t b;


uint16_t ms5611_prom(uint8_t coef_num)
{
  static uint8_t ret;
  static unsigned int rC=0; 
  static uint8_t CMD = 0x00;
  
  SS1_SetLow;                                   //       pull       CSB       low       
  spi_send(CMD_PROM_RD+coef_num*2);       

  // send PROM READ command 
  //  spi_send(0x00);                          // send 0 to read the MSB 
  //  ret=SPI1BUF;       
  HAL_SPI_TransmitReceive(&hspi2, &CMD, &ret, 1, 1000);
  rC=256*ret;  
  HAL_SPI_TransmitReceive(&hspi2, &CMD, &ret, 1, 1000);
  //   spi_send(0x00);                          // send 0 to read the LSB 
  //    ret=SPI1BUF;       
  rC=rC+ret;       
  SS1_SetHigh;                                   //       pull       CSB       high       
  return       rC; 
}

void MS5611_init(void)
{
  
  HAL_GPIO_WritePin(MS5611_CS_PORT, MS5611_CS_PIN, GPIO_PIN_RESET);
  HAL_Delay(1);
  b = CMD_RESET;
  HAL_SPI_Transmit(&hspi2, &b, 1, 1000);

  HAL_GPIO_WritePin(MS5611_CS_PORT, MS5611_CS_PIN, GPIO_PIN_SET);
  return;
  
}

uint32_t MS5611_cmd_adc(uint8_t cmd) 
{ 
  uint8_t ret; 
  static uint32_t temp=0; 
  static uint8_t CMD = 0x00;
  
  SS1_SetLow;                                   // pull CSB low       
  spi_send(CMD_ADC_CONV+cmd);                     // send conversion command    
 
  
  switch (cmd & 0x0f)                             // wait necessary conversion time 
  {       
  case CMD_ADC_256 : HAL_Delay(1); break; 
  case CMD_ADC_512 : HAL_Delay(3);   break; 
  case CMD_ADC_1024: HAL_Delay(4);   break; 
  case CMD_ADC_2048: HAL_Delay(6);   break; 
  case CMD_ADC_4096: HAL_Delay(10);  break; 
  }       
  SS1_SetHigh;                                // pull CSB high to finish the conversion 
  HAL_Delay(10);
  SS1_SetLow;                                // pull CSB low to start new command 
  spi_send(CMD_ADC_READ);                  // send ADC read command 

  // spi_send(0x00);                          // send 0 to read 1st byte (MSB) 
  //  ret=SPI1BUF;       
  HAL_SPI_TransmitReceive(&hspi2, &CMD, &ret, 1, 1000);
  temp=65536*ret; 
  
  // spi_send(0x00);                          // send 0 to read 2nd byte 
  // ret=SPI1BUF;   
  HAL_SPI_TransmitReceive(&hspi2, &CMD, &ret, 1, 1000);
  temp=temp+256*ret; 
  
  // spi_send(0x00);                          // send 0 to read 3rd byte (LSB) 
  //  ret=SPI1BUF;       
  HAL_SPI_TransmitReceive(&hspi2, &CMD, &ret, 1, 1000);
  
  temp=temp+ret;              
  SS1_SetHigh;                                // pull CSB high to finish the read command 
  return       temp;       
}

uint8_t MS5611_crc4(uint16_t n_prom[])
{
  int16_t cnt; // simple counter
  uint16_t n_rem; // crc reminder
  uint16_t crc_read; // original value of the crc
  uint8_t n_bit;
  n_rem = 0x00;
  crc_read=n_prom[7]; //save read CRC
  n_prom[7]=(0xFF00 & (n_prom[7])); //CRC byte is replaced by 0
  for (cnt = 0; cnt < 16; cnt++) // operation is performed on bytes
  { // choose LSB or MSB
    if (cnt%2==1) n_rem ^= (uint16_t) ((n_prom[cnt>>1]) & 0x00FF);
    else n_rem ^= (uint16_t) (n_prom[cnt>>1]>>8);
    for (n_bit = 8; n_bit > 0; n_bit--)
    {
      if (n_rem & (0x8000))
      {
        n_rem = (n_rem << 1) ^ 0x3000;
      }
      else
      {
        n_rem = (n_rem << 1);
      }
    }
  }
  n_rem= (0x000F & (n_rem >> 12)); // // final 4-bit reminder is CRC code
  n_prom[7]=crc_read; // restore the crc_read to its original place
  return (n_rem ^ 0x00);
} 

uint16_t  MS5611_cmd_prom(uint8_t coef_num) 
{ 
 static uint8_t ret;
  static unsigned int rC=0; 
  static uint8_t CMD = 0x00;
  
  SS1_SetLow;   
  HAL_Delay(100);//       pull       CSB       low       
  spi_send(CMD_PROM_RD+coef_num*2);       
 // HAL_Delay(100);
  // send PROM READ command 
  //  spi_send(0x00);                          // send 0 to read the MSB 
  //  ret=SPI1BUF;       
  HAL_SPI_TransmitReceive(&hspi2, &CMD, &ret, 1, 1000);
  rC=256*ret;  
  HAL_SPI_TransmitReceive(&hspi2, &CMD, &ret, 1, 1000);
  //   spi_send(0x00);                          // send 0 to read the LSB 
  //    ret=SPI1BUF;       
  rC=rC+ret;       
  SS1_SetHigh;                                   //       pull       CSB       high       
  return       rC; 
}

void ms5611_calculate(int32_t *pressure, int32_t *temperature)
{
  uint32_t press;
  int64_t temp;
  int64_t delt;
  int64_t dT = (int64_t)ms5611_ut - ((uint64_t)ms5611_c[5] * 256);
  int64_t off = ((int64_t)ms5611_c[2] << 16) + (((int64_t)ms5611_c[4] * dT) >> 7);
  int64_t sens = ((int64_t)ms5611_c[1] << 15) + (((int64_t)ms5611_c[3] * dT) >> 8);
  temp = 2000 + ((dT * (int64_t)ms5611_c[6]) >> 23);
  
  if (temp < 2000) { // temperature lower than 20degC
    delt = temp - 2000;
    delt = 5 * delt * delt;
    off -= delt >> 1;
    sens -= delt >> 2;
    if (temp < -1500) { // temperature lower than -15degC
      delt = temp + 1500;
      delt = delt * delt;
      off -= 7 * delt;
      sens -= (11 * delt) >> 1;
    }
    temp -= ((dT * dT) >> 31);
  }
  press = ((((int64_t)ms5611_up * sens) >> 21) - off) >> 15;
  
  
  if (pressure)
    *pressure = press;
  if (temperature)
    *temperature = temp;
}