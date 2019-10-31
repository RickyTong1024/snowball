#include "stdio.h"
#include <string>
#include <vector>
#include <list>
#include "windows.h"

struct Itstr
{
	int disc;
	std::string type;
	std::string name;
};

struct Files
{
	std::string yu;
	std::string cname;
	std::vector<Itstr> itstr_vec;
	bool use_datetime;
};

std::vector<Files> files_vec_;

int read_proto(std::vector<std::string> ffiles)
{
	for (int i = 0; i < ffiles.size(); ++i)
	{
		Files files;
		files.use_datetime = false;
		FILE *fp = fopen(ffiles[i].c_str(), "r");
		if (!fp)
		{
			return -1;
		}

		char tmpc[100];
		while (fscanf(fp, "%s", &tmpc) != EOF)
		{
			std::string tmps(tmpc);
			if (tmps == "package")
			{
				char c[100];
				fscanf(fp, "%s", &c);
				files.yu = c;
				if (files.yu[files.yu.length() - 1] == ';')
				{
					files.yu = files.yu.substr(0, files.yu.length() - 1);
				}
			}
			else if (tmps == "message")
			{
				char c[100];
				fscanf(fp, "%s", &c);
				files.cname = c;
			}
			else if (tmps == "required")
			{
				Itstr it;
				it.disc = 0;
				char c[100];
				fscanf(fp, "%s", &c);
				it.type = c;
				if (it.type != "bytes" && it.type != "string")
				{
					it.type += "_t";
				}
				fscanf(fp, "%s", &c);
				it.name = c;
				files.itstr_vec.push_back(it);
			}
			else if (tmps == "optional")
			{
				Itstr it;
				it.disc = 0;
				char c[100];
				fscanf(fp, "%s", &c);
				it.type = c;
				if (it.type != "bytes" && it.type != "string")
				{
					it.type += "_t";
				}
				fscanf(fp, "%s", &c);
				it.name = c;
				files.itstr_vec.push_back(it);
			}
			else if (tmps == "repeated")
			{
				Itstr it;
				it.disc = 1;
				char c[100];
				fscanf(fp, "%s", &c);
				it.type = c;
				if (it.type != "bytes" && it.type != "string")
				{
					it.type += "_t";
				}
				fscanf(fp, "%s", &c);
				it.name = c;
				files.itstr_vec.push_back(it);
			}
			else if (tmps == "//using")
			{
				char c[100];
				fscanf(fp, "%s", &c);
				std::string ccc(c);
				if (ccc == "datetime")
				{
					files.use_datetime = true;
				}
			}
		}

		fclose(fp);

		files_vec_.push_back(files);
	}

	return 0;
}

int write_h(const std::string &path)
{
	FILE *fp = fopen(path.c_str(), "w");
	if (!fp)
	{
		return -1;
	}



	fprintf(fp, "#ifndef __SQLQUERY_H__\n");
	fprintf(fp, "#define __SQLQUERY_H__\n");
	fprintf(fp, "\n");
	fprintf(fp, "#include \"service_inc.h\"\n");
	fprintf(fp, "#include \"mysql++.h\"\n");
	fprintf(fp, "\n");
	fprintf(fp, "class SqlQuery\n");
	fprintf(fp, "{\n");
	fprintf(fp, "public:\n");
	fprintf(fp, "	SqlQuery(uint64_t guid, google::protobuf::Message *data) : guid_(guid), data_(data) {}");
	fprintf(fp, "\n");
	fprintf(fp, "	virtual int insert(mysqlpp::Connection *conn) = 0;\n");
	fprintf(fp, "	virtual int query(mysqlpp::Connection *conn) = 0;\n");
	fprintf(fp, "	virtual int update(mysqlpp::Connection *conn) = 0;\n");
	fprintf(fp, "	virtual int remove(mysqlpp::Connection *conn) = 0;\n");
	fprintf(fp, "\n");
	fprintf(fp, "protected:\n");
	fprintf(fp, "	google::protobuf::Message *data_;\n");
	fprintf(fp, "	uint64_t guid_;\n");
	fprintf(fp, "};\n");
	fprintf(fp, "\n");
	for (int i = 0; i < files_vec_.size(); ++i)
	{
		fprintf(fp, "class Sql%s : public SqlQuery\n", files_vec_[i].cname.c_str());
		fprintf(fp, "{\n");
		fprintf(fp, "public:\n");
		fprintf(fp, "	Sql%s(uint64_t guid, google::protobuf::Message *data) : SqlQuery(guid, data) {}\n", files_vec_[i].cname.c_str());
		fprintf(fp, "	virtual int insert(mysqlpp::Connection *conn);\n");
		fprintf(fp, "	virtual int query(mysqlpp::Connection *conn);\n");
		fprintf(fp, "	virtual int update(mysqlpp::Connection *conn);\n");
		fprintf(fp, "	virtual int remove(mysqlpp::Connection *conn);\n");
		fprintf(fp, "};\n");
		fprintf(fp, "\n");
	}
	fprintf(fp, "#endif\n");

	fclose(fp);

	return 0;
}

int write_cpp(const std::string &path)
{
	FILE *fp = fopen(path.c_str(), "w");
	if (!fp)
	{
		return -1;
	}

	/// 头部
	fprintf(fp, "#include \"sqlquery.h\"\n");
	fprintf(fp, "#include <sstream>\n");
	fprintf(fp, "#include \"protocol_inc.h\"\n");
	fprintf(fp, "\n");

	/// 类实现
	for (int i = 0; i < files_vec_.size(); ++i)
	{
		/// insert
		fprintf(fp, "int Sql%s::insert(mysqlpp::Connection *conn)\n", files_vec_[i].cname.c_str());
		fprintf(fp, "{\n");
		fprintf(fp, "	mysqlpp::Query query = conn->query();\n");
		fprintf(fp, "	%s::%s *obj = (%s::%s *)data_;\n", files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str(), files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str());
		fprintf(fp, "	query << \"INSERT INTO %s SET \";\n", files_vec_[i].cname.c_str());
		for (int j = 0; j < files_vec_[i].itstr_vec.size(); ++j)
		{
			if (files_vec_[i].itstr_vec[j].disc == 0)
			{
				if (files_vec_[i].itstr_vec[j].type != "bytes" && files_vec_[i].itstr_vec[j].type != "string")
				{
					fprintf(fp, "	query << \"%s=\" << boost::lexical_cast<std::string>(obj->%s());\n", files_vec_[i].itstr_vec[j].name.c_str(), files_vec_[i].itstr_vec[j].name.c_str());
				}
				else
				{
					fprintf(fp, "	query << \"%s=\" << mysqlpp::quote << obj->%s();\n", files_vec_[i].itstr_vec[j].name.c_str(), files_vec_[i].itstr_vec[j].name.c_str());
				}
			}
			else
			{
				if (files_vec_[i].itstr_vec[j].type != "bytes" && files_vec_[i].itstr_vec[j].type != "string")
				{
					fprintf(fp, "	{\n");
					fprintf(fp, "		uint32_t size = obj->%s_size();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "		std::stringstream ssm;\n");
					fprintf(fp, "		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));\n");
					fprintf(fp, "		for (uint32_t i = 0; i < size; i++)\n");
					fprintf(fp, "		{\n");
					fprintf(fp, "			%s v = obj->%s(i);\n", files_vec_[i].itstr_vec[j].type.c_str(), files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "			ssm.write(reinterpret_cast<char*>(&v), sizeof(%s));\n", files_vec_[i].itstr_vec[j].type.c_str());
					fprintf(fp, "		}\n");
					fprintf(fp, "		query << \"%s=\" << mysqlpp::quote << ssm.str();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "	}\n");
				}
				else
				{
					fprintf(fp, "	{\n");
					fprintf(fp, "		uint32_t size = obj->%s_size();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "		std::stringstream ssm;\n");
					fprintf(fp, "		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));\n");
					fprintf(fp, "		for (uint32_t i = 0; i < size; i++)\n");
					fprintf(fp, "		{\n");
					fprintf(fp, "			std::string v = obj->%s(i);\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "			uint32_t len = v.size() + 1;\n");
					fprintf(fp, "			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));\n");
					fprintf(fp, "			ssm.write(v.data(), len);\n");
					fprintf(fp, "		}\n");
					fprintf(fp, "		query << \"%s=\" << mysqlpp::quote << ssm.str();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "	}\n");
				}
			}
			if (j != files_vec_[i].itstr_vec.size() - 1)
			{
				fprintf(fp, "	query << \",\";\n");
			}
			else if (files_vec_[i].use_datetime)
			{
				fprintf(fp, "	query << \",\";\n");
				fprintf(fp, "	query << \"dt=now()\";\n");
			}
		}
		fprintf(fp, "\n");
		fprintf(fp, "	mysqlpp::SimpleResult res = query.execute();\n");
		fprintf(fp, "\n");
		fprintf(fp, "	if (!res)\n");
		fprintf(fp, "	{\n");
		fprintf(fp, "		service::log()->error(query.error());\n");
		fprintf(fp, "		return -1;\n");
		fprintf(fp, "	}\n");
		fprintf(fp, "	return 0;\n");
		fprintf(fp, "}\n");
		fprintf(fp, "\n");

		/// query
		fprintf(fp, "int Sql%s::query(mysqlpp::Connection *conn)\n", files_vec_[i].cname.c_str());
		fprintf(fp, "{\n");
		fprintf(fp, "	mysqlpp::Query query = conn->query();\n");
		fprintf(fp, "	%s::%s *obj = (%s::%s *)data_;\n", files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str(), files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str());
		fprintf(fp, "	query << \"SELECT * FROM %s WHERE guid=\"\n", files_vec_[i].cname.c_str());
		fprintf(fp, "		<< boost::lexical_cast<std::string>(guid_);\n");
		fprintf(fp, "\n");
		fprintf(fp, "	mysqlpp::StoreQueryResult res = query.store();\n");
		fprintf(fp, "\n");
		fprintf(fp, "	if (!res || res.num_rows() != 1)\n");
		fprintf(fp, "	{\n");
		fprintf(fp, "		return -1;\n");
		fprintf(fp, "	}\n");
		fprintf(fp, "\n");
		for (int j = 0; j < files_vec_[i].itstr_vec.size(); ++j)
		{
			if (files_vec_[i].itstr_vec[j].disc == 0)
			{
				if (files_vec_[i].itstr_vec[j].type != "bytes" && files_vec_[i].itstr_vec[j].type != "string")
				{
					fprintf(fp, "	if (!res.at(0).at(%d).is_null())\n", j);
					fprintf(fp, "	{\n");
					fprintf(fp, "		obj->set_%s(res.at(0).at(%d));\n", files_vec_[i].itstr_vec[j].name.c_str(), j);
					fprintf(fp, "	}\n");
				}
				else
				{
					fprintf(fp, "	if (!res.at(0).at(%d).is_null())\n", j);
					fprintf(fp, "	{\n");
					fprintf(fp, "		obj->set_%s((std::string)res.at(0).at(%d));\n", files_vec_[i].itstr_vec[j].name.c_str(), j);
					fprintf(fp, "	}\n");
				}
			}
			else
			{
				if (files_vec_[i].itstr_vec[j].type != "bytes" && files_vec_[i].itstr_vec[j].type != "string")
				{
					fprintf(fp, "	if (!res.at(0).at(%d).is_null())\n", j);
					fprintf(fp, "	{\n");
					fprintf(fp, "		std::string temp(res.at(0).at(%d).data(), res.at(0).at(%d).length());\n", j, j);
					fprintf(fp, "		std::stringstream ssm(temp);\n");
					fprintf(fp, "		uint32_t size = 0;\n");
					fprintf(fp, "		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));\n");
					fprintf(fp, "		%s v;\n", files_vec_[i].itstr_vec[j].type.c_str());
					fprintf(fp, "		for (uint32_t i = 0; i < size; i++)\n");
					fprintf(fp, "		{\n");
					fprintf(fp, "			ssm.read(reinterpret_cast<char*>(&v), sizeof(%s));\n", files_vec_[i].itstr_vec[j].type.c_str());
					fprintf(fp, "			obj->add_%s(v);\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "		}\n");
					fprintf(fp, "	}\n");
				}
				else
				{
					fprintf(fp, "	if (!res.at(0).at(%d).is_null())\n", j);
					fprintf(fp, "	{\n");
					fprintf(fp, "		std::string temp(res.at(0).at(%d).data(), res.at(0).at(%d).length());\n", j, j);
					fprintf(fp, "		std::stringstream ssm(temp);\n");
					fprintf(fp, "		uint32_t size = 0;\n");
					fprintf(fp, "		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));\n");
					fprintf(fp, "		uint32_t len = 0;\n");
					fprintf(fp, "		for (uint32_t i = 0; i < size; i++)\n");
					fprintf(fp, "		{\n");
					fprintf(fp, "			ssm.read(reinterpret_cast<char*>(&len), sizeof(uint32_t));\n");
					fprintf(fp, "			boost::scoped_array<char> buf(new char[len]);\n");
					fprintf(fp, "			ssm.read(buf.get(), len);\n");
					fprintf(fp, "			obj->add_%s(buf.get(), len);\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "		}\n");
					fprintf(fp, "	}\n");
				}
			}
		}
		fprintf(fp, "	return 0;\n");
		fprintf(fp, "}\n");
		fprintf(fp, "\n");


		/// update
		fprintf(fp, "int Sql%s::update(mysqlpp::Connection *conn)\n", files_vec_[i].cname.c_str());
		fprintf(fp, "{\n");
		fprintf(fp, "	mysqlpp::Query query = conn->query();\n");
		fprintf(fp, "	%s::%s *obj = (%s::%s *)data_;\n", files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str(), files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str());
		fprintf(fp, "	query << \"UPDATE %s SET \";\n", files_vec_[i].cname.c_str());
		for (int j = 0; j < files_vec_[i].itstr_vec.size(); ++j)
		{
			if (files_vec_[i].itstr_vec[j].disc == 0)
			{
				if (files_vec_[i].itstr_vec[j].type != "bytes" && files_vec_[i].itstr_vec[j].type != "string")
				{
					fprintf(fp, "	query << \"%s=\" << boost::lexical_cast<std::string>(obj->%s());\n", files_vec_[i].itstr_vec[j].name.c_str(), files_vec_[i].itstr_vec[j].name.c_str());
				}
				else
				{
					fprintf(fp, "	query << \"%s=\" << mysqlpp::quote << obj->%s();\n", files_vec_[i].itstr_vec[j].name.c_str(), files_vec_[i].itstr_vec[j].name.c_str());
				}
			}
			else
			{
				if (files_vec_[i].itstr_vec[j].type != "bytes" && files_vec_[i].itstr_vec[j].type != "string")
				{
					fprintf(fp, "	{\n");
					fprintf(fp, "		uint32_t size = obj->%s_size();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "		std::stringstream ssm;\n");
					fprintf(fp, "		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));\n");
					fprintf(fp, "		for (uint32_t i = 0; i < size; i++)\n");
					fprintf(fp, "		{\n");
					fprintf(fp, "			%s v = obj->%s(i);\n", files_vec_[i].itstr_vec[j].type.c_str(), files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "			ssm.write(reinterpret_cast<char*>(&v), sizeof(%s));\n", files_vec_[i].itstr_vec[j].type.c_str());
					fprintf(fp, "		}\n");
					fprintf(fp, "		query << \"%s=\" << mysqlpp::quote << ssm.str();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "	}\n");
				}
				else
				{
					fprintf(fp, "	{\n");
					fprintf(fp, "		uint32_t size = obj->%s_size();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "		std::stringstream ssm;\n");
					fprintf(fp, "		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));\n");
					fprintf(fp, "		for (uint32_t i = 0; i < size; i++)\n");
					fprintf(fp, "		{\n");
					fprintf(fp, "			std::string v = obj->%s(i);\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "			uint32_t len = v.size() + 1;\n");
					fprintf(fp, "			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));\n");
					fprintf(fp, "			ssm.write(v.data(), len);\n");
					fprintf(fp, "		}\n");
					fprintf(fp, "		query << \"%s=\" << mysqlpp::quote << ssm.str();\n", files_vec_[i].itstr_vec[j].name.c_str());
					fprintf(fp, "	}\n");
				}
			}
			if (j != files_vec_[i].itstr_vec.size() - 1)
			{
				fprintf(fp, "	query << \",\";\n");
			}
		}
		fprintf(fp, "	query << \" WHERE guid=\" << boost::lexical_cast<std::string>(guid_);\n");
		fprintf(fp, "\n");
		fprintf(fp, "	mysqlpp::SimpleResult res = query.execute();\n");
		fprintf(fp, "\n");
		fprintf(fp, "	if (!res)\n");
		fprintf(fp, "	{\n");
		fprintf(fp, "		service::log()->error(query.error());\n");
		fprintf(fp, "		return -1;\n");
		fprintf(fp, "	}\n");
		fprintf(fp, "	return 0;\n");
		fprintf(fp, "}\n");
		fprintf(fp, "\n");


		/// remove
		fprintf(fp, "int Sql%s::remove(mysqlpp::Connection *conn)\n", files_vec_[i].cname.c_str());
		fprintf(fp, "{\n");
		fprintf(fp, "	mysqlpp::Query query = conn->query();\n");
		fprintf(fp, "	%s::%s *obj = (%s::%s *)data_;\n", files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str(), files_vec_[i].yu.c_str(), files_vec_[i].cname.c_str());
		fprintf(fp, "	query << \"DELETE FROM %s WHERE guid=\"\n", files_vec_[i].cname.c_str());
		fprintf(fp, "		<< boost::lexical_cast<std::string>(guid_);\n");
		fprintf(fp, "\n");
		fprintf(fp, "	mysqlpp::SimpleResult res = query.execute();\n");
		fprintf(fp, "\n");
		fprintf(fp, "	if (!res)\n");
		fprintf(fp, "	{\n");
		fprintf(fp, "		service::log()->error(query.error());\n");
		fprintf(fp, "		return -1;\n");
		fprintf(fp, "	}\n");
		fprintf(fp, "	return 0;\n");
		fprintf(fp, "}\n");

		/// 分割线
		if (i != files_vec_.size() - 1)
		{
			fprintf(fp, "\n");
			fprintf(fp, "//////////////////////////////////////////////////////////////////////////\n");
			fprintf(fp, "\n");
		}
	}

	fclose(fp);

	return 0;
}

int main(int argc, char *argv[])
{
	if (argc < 4)
	{
		return 0;
	}
	std::vector<std::string> files;
	for (int i = 3; i < argc; ++i)
	{
		files.push_back(argv[i]);
	}
	if (read_proto(files) == -1)
	{
		return -1;
	}
	if (write_h(argv[1]) == -1)
	{
		return -1;
	}
	if (write_cpp(argv[2]) == -1)
	{
		return -1;
	}
	return 0;
}
