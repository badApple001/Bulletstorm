#include "pch.h"
#include "Native.h"
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <fstream>
#include <sys/stat.h>
#include <random>
#include <climits>
#include <sstream>
#include <direct.h>
#include "File.hpp"
using namespace std;



static random_device s_seed; //Ӳ             
static ranlux48 s_randomEngine(s_seed()); //                     
static uniform_int_distribution<unsigned int> s_uint_distrib(UINT_MAX / 2, UINT_MAX);
static LogCallback s_logCallback = NULL;
static char version[] = "1.0.0";

unsigned int get_random_uint() {
	return s_uint_distrib(s_randomEngine);
}


void CopyTo(const char* s, const char* d) {
	std::ifstream  src(s, std::ios::binary);
	std::ofstream  dst(d, std::ios::binary);
	dst << src.rdbuf();
}


bool Contain(const char* path, const char* dest) {
	std::ifstream infile(path, std::ios::in | std::ios::binary | std::ios::ate);
	size_t size = infile.tellg();
	infile.seekg(0, std::ios::beg);
	char* buffer = new char[size];
	infile.read(buffer, size);
	infile.close();
	std::string alltext(buffer, size);
	delete[] buffer;
	return alltext.find(dest) != std::string::npos;
}


//   ܻ        ߼ 
char* encrtypt_file(char* src, size_t& file_size) {

	//     Կ    
	uniform_int_distribution<int> key_distrib(130, 140);
	int kl = key_distrib(s_randomEngine);

	//     Կ      ָ  
	unsigned int* p_passwordArr = new unsigned int[kl];
	for (int i = 0; i < kl; i++)
	{
		p_passwordArr[i] = get_random_uint();
	}

	//             λ  intָ  
	int klsize = (kl + 1) * sizeof(uint32_t);
	const int safe_size = 1024;//  ȫ    С
	//        С
	const size_t encrtypt_size = file_size - safe_size;
	//    һ   µ  ڴ濨              Դ ļ 
	char* des = (char*)malloc(file_size + klsize);
	//    ȫ      Cpy   µ  ڴ  
	memcpy(des, src, safe_size);

	//      ָ  
	unsigned int* da = (unsigned int*)(des + safe_size);
	//    Դ    ָ  
	unsigned int* db = (unsigned int*)(src + safe_size);
	//         ĸ  ֽڵ ʮ  λΪ           鳤  
	*(da++) = (get_random_uint() & 0xFFFF0000) | (kl & 0xFFFF);
	//д        
	memcpy(da, p_passwordArr, kl * sizeof(uint32_t));
	//ָ       
	da += kl;

	for (size_t i = 0; i < encrtypt_size; i += 4) {
		int index = (i + (i / kl)) % kl;
		da[i / 4] = p_passwordArr[index] ^ db[i / 4];
	}

	file_size += klsize;
	delete[] p_passwordArr;
	return des;
}

//   ת  С  
int get_little_endian(unsigned int x) {
	return ((x >> 24) & 0xff) | ((x << 8) & 0xff0000) | ((x >> 8) & 0xff00) | ((x << 24) & 0xff000000);
}

// ļ  Ƿ    
bool file_exist(const char* path) {
	struct stat _Stat;
	if (stat(path, &_Stat) != 0) {

		return false;
	}
	return true;
}

//     ־
void Log(const char* fmt, ...) {
	if (s_logCallback == NULL)return;
	char acLogStr[512];// = { 0 };
	va_list ap;
	va_start(ap, fmt);
	vsprintf(acLogStr, fmt, ap);
	va_end(ap);
	s_logCallback(acLogStr, strlen(acLogStr));
}

// ⲿ    
void OverrideLoader(char* path) {

	Log("override Loader code...");

	char* workpath = _getcwd(NULL, 0);
	Log(workpath);

	if (NULL == path || strcmp(path, "") == 0) {
		free(workpath);
		workpath = NULL;
		return;
	}

	string localpath = workpath;
	string new_loaderCpp = localpath + "\\Assets/EasyObfuscation/Editor/local/vm_MetadataLoader.cpp";
	string new_memoryMappedFileH = localpath + "\\Assets/EasyObfuscation/Editor/local/utils_MemoryMappedFile.h";
	string new_memoryMappedFileCpp = localpath + "\\Assets/EasyObfuscation/Editor/local/utils_MemoryMappedFile.cpp";

	if (!file_exist(new_loaderCpp.c_str()))
	{
		Log("miss: %s", new_loaderCpp.c_str());
		return;
	}
	if (!file_exist(new_memoryMappedFileH.c_str()))
	{
		Log("miss: %s", new_memoryMappedFileH.c_str());
		return;
	}
	if (!file_exist(new_memoryMappedFileCpp.c_str()))
	{
		Log("miss: %s", new_memoryMappedFileCpp.c_str());
		return;
	}

	string dest = path;
	string dest_loaderCpp = dest + "\\unityLibrary\\src\\main\\Il2CppOutputProject\\IL2CPP\\libil2cpp\\vm\\MetadataLoader.cpp";
	string dest_memoryMappedFileH = dest + "\\unityLibrary\\src\\main\\Il2CppOutputProject\\IL2CPP\\libil2cpp\\utils\\MemoryMappedFile.h";
	string dest_memoryMappedFileCpp = dest + "\\unityLibrary\\src\\main\\Il2CppOutputProject\\IL2CPP\\libil2cpp\\utils\\MemoryMappedFile.cpp";
	if (!file_exist(dest_loaderCpp.c_str()))
	{
		Log("error: not found dest loaderCpp");
		return;
	}
	if (!file_exist(dest_memoryMappedFileH.c_str()))
	{
		Log("error: not found dest memoryMapped head file");
		return;
	}
	if (!file_exist(dest_memoryMappedFileCpp.c_str()))
	{
		Log("error: not found dest memoryMapped source file");
		return;
	}
	if (File::Extract((dest + "\\unityLibrary\\src\\main\\assets\\bin\\Data\\data.unity3d").c_str(), 0x12, 11) != "2021.3.22f1") {
		//   ⲻͬ 汾il2cppԴ 벻ͬ    ¿                       ϻ          
		Log("version erro: must be 2021.3.22f1");
		return;
	}

	CopyTo(new_loaderCpp.c_str(), dest_loaderCpp.c_str());
	CopyTo(new_memoryMappedFileCpp.c_str(), dest_memoryMappedFileCpp.c_str());
	CopyTo(new_memoryMappedFileH.c_str(), dest_memoryMappedFileH.c_str());

	if (workpath != NULL) {
		free(workpath);
		workpath = NULL;
	}


	Log("owner: 31games");
	Log("anchor: https://geek7.blog.csdn.net/");
	Log("email: isysprey@foxmail.com");
	Log("override Loader code complete.");
}

// ⲿ    
void EncryptionCode(char* export_android_path)
{
	if (NULL == export_android_path || strcmp(export_android_path, "") == 0) {
		Log("export android path is null: %s", export_android_path);
		return;
	}

	//wait input
	string global_metadata_path = export_android_path;
	if (NULL == strstr(global_metadata_path.c_str(), "global-metadata.dat")) {
		global_metadata_path += "\\unityLibrary\\src\\main\\assets\\bin\\Data\\Managed\\Metadata\\global-metadata.dat";
	}

	//cheack file vaild
	if (!file_exist(global_metadata_path.c_str())) {
		Log("file not found: %s\n", global_metadata_path);
		return;
	}

	Log("EasyObfuscation version: %s", version);

	//load file
	ifstream infile(global_metadata_path, ios::in | ios::binary | ios::ate);
	size_t size = infile.tellg();
	infile.seekg(0, ios::beg);
	char* buffer = new char[size];
	infile.read(buffer, size);
	infile.close();

	//encrtypt
	size_t srcsize = size;
	char* encbuffer = encrtypt_file(buffer, size);
	ofstream outfile(global_metadata_path, ios::out | ios::binary | ios::ate);
	if (!outfile) {
		Log("open file fail: %s\n", global_metadata_path);
		return;
	}

	//log
	unsigned int* hex_buffer = (unsigned int*)buffer;
	unsigned int* hex_encbuffer = (unsigned int*)encbuffer;
	unsigned int src_value = get_little_endian(*hex_buffer);
	unsigned int enc_value = get_little_endian(*hex_encbuffer);
	Log("src: %x\tsrc buffer size: %ld\nenc: %x\tenc buffer size: %ld", src_value, srcsize, enc_value, size);

	outfile.write(encbuffer, size);
	outfile.close();
	delete[] buffer;
	free(encbuffer);

	Log("call cpp complete.");
}

//  ־ ص  ӿ 
void SetDisplayLog(LogCallback callback)
{
	s_logCallback = callback;
}
