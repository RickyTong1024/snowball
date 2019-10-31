#include "stdio.h"
#include <string>
#include <vector>
#include <list>
#include <map>
#include <algorithm>
#include "windows.h"
#include <mysql++.h>
#include <ssqls.h>
#include <boost/lexical_cast.hpp>

struct Itstr
{
	int disc;
	std::string type;
	std::string name;
};

struct Indexstr
{
	std::string index_name;
	int index_len;
};

struct Files
{
	std::string cname;
	std::vector<Itstr> itstr_vec;
	bool use_datetime;
	bool use_auto_increment;
	std::vector<Indexstr> uniques;
	std::vector<Indexstr> indexes;
	
	bool operator == (const std::string &name)
	{
		return cname == name;
	}

	bool operator == (const Files & rhs)
	{
		return cname == rhs.cname;
	}

	bool operator != (const Files & rhs)
	{
		return !(operator == (rhs));
	}

	bool operator != (const std::string &name)
	{
		return !(operator == (name));
	}
};

std::vector<Files> files_vec_;
std::map<std::string, std::vector<std::string> > field_map_;

int read_proto(const std::string &path)
{
	Files files;
	files.use_auto_increment = false;
	files.use_datetime = false;
	FILE *fp = fopen(path.c_str(), "r");
	if (!fp)
	{
		return -1;
	}
	//add
	std::vector<std::string> field_name;
	char tmpc[100];
	while (fscanf(fp, "%s", &tmpc) != EOF)
	{
		std::string tmps(tmpc);
		if (tmps == "message")
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
			fscanf(fp, "%s", &c);
			it.name = c;
			//add
			field_name.push_back(it.name);
			files.itstr_vec.push_back(it);
		}
		else if (tmps == "optional")
		{
			Itstr it;
			it.disc = 0;
			char c[100];
			fscanf(fp, "%s", &c);
			it.type = c;
			fscanf(fp, "%s", &c);
			it.name = c;
			//add
			field_name.push_back(it.name);
			files.itstr_vec.push_back(it);
		}
		else if (tmps == "repeated")
		{
			Itstr it;
			it.disc = 1;
			char c[100];
			fscanf(fp, "%s", &c);
			it.type = c;
			fscanf(fp, "%s", &c);
			it.name = c;
			//add
			field_name.push_back(it.name);
			files.itstr_vec.push_back(it);
		}
		else if (tmps == "//using")
		{
			char c[100];
			fscanf(fp, "%s", &c);
			std::string ccc(c);
			if (ccc == "autoinc")
			{
				files.use_auto_increment = true;
			}
			else if (ccc == "datetime")
			{
				files.use_datetime = true;
			}
			else if (ccc == "unique")
			{
				fscanf(fp, "%s", &c);
				std::string in1(c);
				fscanf(fp, "%s", &c);
				std::string in2(c);
				Indexstr ist;
				ist.index_name = in1;
				ist.index_len = boost::lexical_cast<int>(in2);
				files.uniques.push_back(ist);
			}
			else if (ccc == "index")
			{
				fscanf(fp, "%s", &c);
				std::string in1(c);
				fscanf(fp, "%s", &c);
				std::string in2(c);
				Indexstr ist;
				ist.index_name = in1;
				ist.index_len = boost::lexical_cast<int>(in2);
				files.indexes.push_back(ist);
			}
		}
	}

	fclose(fp);

	if (files.use_datetime)
	{
		Itstr it;
		it.disc = 0;
		char c[100];
		it.type = "dt";
		it.name = "dt";
		//add
		field_name.push_back(it.name);
		files.itstr_vec.push_back(it);
	}

	files_vec_.push_back(files);
	field_map_[files.cname] = field_name;

	return 0;
}

int write_sql(const std::string &db, const std::string &ip, const std::string &user, const std::string &pwd)
{
	mysqlpp::Connection *conn = new mysqlpp::Connection;
	conn->disable_exceptions ();
	int ok = conn->connect(db.c_str(), ip.c_str(), user.c_str(), pwd.c_str(), 3306);
	if (!ok)
	{
		return -1;
	}

	//添加protocol
	for (int i = 0; i < files_vec_.size(); ++i)
	{
		mysqlpp::Query query = conn->query();
		query << "DROP TABLE ";
		query << files_vec_[i].cname;

		query.execute();

		//////////////////////////////////////////////////////////////////////////

		mysqlpp::Query query1 = conn->query();
		query1 << "CREATE TABLE ";
		query1 << files_vec_[i].cname;
		query1 << " (";
		
		for (int j = 0; j < files_vec_[i].itstr_vec.size(); ++j)
		{
			query1 << files_vec_[i].itstr_vec[j].name;
			query1 << " ";
			if (files_vec_[i].itstr_vec[j].disc == 1)
			{
				query1 << "mediumblob";
			}
			else if (files_vec_[i].itstr_vec[j].type == "int64")
			{
				query1 << "bigint not null default 0";
			}
			else if (files_vec_[i].itstr_vec[j].type == "uint64")
			{
				query1 << "bigint";
				if (j != 0)
				{
					query1 << " not null default 0";
				}
			}
			else if (files_vec_[i].itstr_vec[j].type == "int32")
			{
				query1 << "int not null default 0";
			}
			else if (files_vec_[i].itstr_vec[j].type == "uint32")
			{
				query1 << "int not null default 0";
			}
			else if (files_vec_[i].itstr_vec[j].type == "bytes")
			{
				query1 << "mediumblob";
			}
			else if (files_vec_[i].itstr_vec[j].type == "string")
			{
				query1 << "text";
			}
			else if (files_vec_[i].itstr_vec[j].type == "dt")
			{
				query1 << "timestamp not null default now()";
			}
			else if (files_vec_[i].itstr_vec[j].type == "float")
			{
				query1 << "double not null default 0";
			}
			else if (files_vec_[i].itstr_vec[j].type == "double")
			{
				query1 << "double not null default 0";
			}

			if (j == 0)
			{
				if (files_vec_[i].use_auto_increment)
				{
					query1 << " auto_increment";
				}
				query1 << " PRIMARY KEY";
			}
			if (j != files_vec_[i].itstr_vec.size() - 1)
			{
				query1 << ", ";
			}
		}
		query1 << ")";

		query1.execute();

		//////////////////////////////////////////////////////////////////////////

		for (int j = 0; j < files_vec_[i].indexes.size(); ++j)
		{
			mysqlpp::Query query2 = conn->query();
			query2 << "CREATE INDEX ";
			query2 << files_vec_[i].itstr_vec[j].name;
			query2 << "_index ON ";
			query2 << files_vec_[i].cname;
			query2 << " (";
			query2 << files_vec_[i].indexes[j].index_name;
			if (files_vec_[i].indexes[j].index_len)
			{
				query2 << "(";
				query2 << boost::lexical_cast<std::string>(files_vec_[i].indexes[j].index_len);
				query2 << ")";
			}
			query2 << ")";

			query2.execute();
		}

		for (int j = 0; j < files_vec_[i].uniques.size(); ++j)
		{
			mysqlpp::Query query2 = conn->query();
			query2 << "ALTER TABLE ";
			query2 << files_vec_[i].cname;
			query2 << " ADD UNIQUE ";
			query2 << " (";
			query2 << files_vec_[i].uniques[j].index_name;
			if (files_vec_[i].uniques[j].index_len)
			{
				query2 << "(";
				query2 << boost::lexical_cast<std::string>(files_vec_[i].uniques[j].index_len);
				query2 << ")";
			}
			query2 << ")";

			query2.execute();
		}
	}

	conn->disconnect();

	return 0;
}

int write_sql2(const std::string &db, const std::string &ip, const std::string &user, const std::string &pwd)
{
	mysqlpp::Connection *conn = new mysqlpp::Connection;
	conn->disable_exceptions ();
	int ok = conn->connect(db.c_str(), ip.c_str(), user.c_str(), pwd.c_str(), 3306);
	if (!ok)
	{
		return -1;
	}	

	//添加protocol
	for (int i = 0; i < files_vec_.size(); ++i)
	{
		std::string table_name = files_vec_[i].cname;
		std::string select_name = "select * from " + table_name + " limit 1";
		mysqlpp::Query query = conn->query(select_name);
		mysqlpp::StoreQueryResult res  = query.store();
		if (res)
		{
			//add
			for (int k = 0; k < res.num_fields(); ++k)
			{
				std::vector<std::string> & names = field_map_[table_name];
				if (std::find(names.begin(), names.end(), res.field_name(k)) == names.end())
				{
					mysqlpp::Query query = conn->query();
					query << "ALTER TABLE ";
					query << table_name ;
					query << " ";
					query << "DROP ";
					query << "COLUMN ";
					query << res.field_name(k);
					query.execute();
				}
			}

			for (int j = 0; j < files_vec_[i].itstr_vec.size(); ++j)
			{
				std::string query_name = "select DATA_TYPE from information_schema.COLUMNS where TABLE_SCHEMA='" + db + "' and TABLE_NAME='" + table_name + "' and column_name = '" + files_vec_[i].itstr_vec[j].name + "'";
				mysqlpp::Query query2 = conn->query(query_name);
				mysqlpp::StoreQueryResult res = query2.store();
				if (res && res.num_rows() == 1)
				{
					if (res[0][0] == "mediumblob" && files_vec_[i].itstr_vec[j].disc == 1)
					{
						continue;
					}
					else if (res[0][0] == "bigint" && files_vec_[i].itstr_vec[j].type == "int64")
					{
						continue;
					}
					else if (res[0][0] == "bigint" && files_vec_[i].itstr_vec[j].type == "uint64")
					{
						continue;
					}
					else if (res[0][0] == "int" && files_vec_[i].itstr_vec[j].type == "int32")
					{
						continue;
					}
					else if (res[0][0] == "int" && files_vec_[i].itstr_vec[j].type == "uint32")
					{
						continue;
					}
					else if (res[0][0] == "mediumblob" && files_vec_[i].itstr_vec[j].type == "bytes")
					{
						continue;
					}
					else if (res[0][0] == "text" && files_vec_[i].itstr_vec[j].type == "string")
					{
						continue;
					}
					else if (res[0][0] == "timestamp" && files_vec_[i].itstr_vec[j].type == "dt")
					{
						continue;
					}
					else if (res[0][0] == "double" && files_vec_[i].itstr_vec[j].type == "float")
					{
						continue;
					}
					else if (res[0][0] == "double" && files_vec_[i].itstr_vec[j].type == "double")
					{
						continue;
					}

					///添加新列
					mysqlpp::Query query3 = conn->query();
					query3 << "ALTER TABLE ";
					query3 << table_name;
					query3 << " ";
					query3 << "MODIFY COLUMN ";
					query3 << files_vec_[i].itstr_vec[j].name;
					query3 << " ";
					if (files_vec_[i].itstr_vec[j].disc == 1)
					{
						query3 << "mediumblob";
					}
					else if (files_vec_[i].itstr_vec[j].type == "int64")
					{
						query3 << "bigint not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "uint64")
					{
						query3 << "bigint";
						if (j != 0)
						{
							query3 << " not null default 0";
						}
					}
					else if (files_vec_[i].itstr_vec[j].type == "int32")
					{
						query3 << "int not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "uint32")
					{
						query3 << "int not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "bytes")
					{
						query3 << "mediumblob";
					}
					else if (files_vec_[i].itstr_vec[j].type == "string")
					{
						query3 << "text";
					}
					else if (files_vec_[i].itstr_vec[j].type == "dt")
					{
						query3 << "timestamp not null default now()";
					}
					else if (files_vec_[i].itstr_vec[j].type == "float")
					{
						query3 << "double not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "double")
					{
						query3 << "double not null default 0";
					}

					if (j != 0)
					{
						query3 << " AFTER ";
						query3 << files_vec_[i].itstr_vec[j - 1].name;
					}
					query3.execute();
				}
				else
				{
					///添加新列
					mysqlpp::Query query3 = conn->query();
					query3 << "ALTER TABLE ";
					query3 << table_name;
					query3 << " ";
					query3 << "ADD COLUMN ";
					query3 << files_vec_[i].itstr_vec[j].name;
					query3 << " ";
					if (files_vec_[i].itstr_vec[j].disc == 1)
					{
						query3 << "mediumblob";
					}
					else if (files_vec_[i].itstr_vec[j].type == "int64")
					{
						query3 << "bigint not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "uint64")
					{
						query3 << "bigint";
						if (j != 0)
						{
							query3 << " not null default 0";
						}
					}
					else if (files_vec_[i].itstr_vec[j].type == "int32")
					{
						query3 << "int not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "uint32")
					{
						query3 << "int not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "bytes")
					{
						query3 << "mediumblob";
					}
					else if (files_vec_[i].itstr_vec[j].type == "string")
					{
						query3 << "text";
					}
					else if (files_vec_[i].itstr_vec[j].type == "dt")
					{
						query3 << "timestamp not null default now()";
					}
					else if (files_vec_[i].itstr_vec[j].type == "float")
					{
						query3 << "double not null default 0";
					}
					else if (files_vec_[i].itstr_vec[j].type == "double")
					{
						query3 << "double not null default 0";
					}

					if (j != 0)
					{
						query3 << " AFTER ";
						query3 << files_vec_[i].itstr_vec[j - 1].name;
					}
					query3.execute();
				}
			}
		}
		else
		{
			mysqlpp::Query query1 = conn->query();
			query1 << "CREATE TABLE ";
			query1 << files_vec_[i].cname;
			query1 << " (";

			for (int j = 0; j < files_vec_[i].itstr_vec.size(); ++j)
			{
				query1 << files_vec_[i].itstr_vec[j].name;
				query1 << " ";
				if (files_vec_[i].itstr_vec[j].disc == 1)
				{
					query1 << "mediumblob";
				}
				else if (files_vec_[i].itstr_vec[j].type == "int64")
				{
					query1 << "bigint not null default 0";
				}
				else if (files_vec_[i].itstr_vec[j].type == "uint64")
				{
					query1 << "bigint";
					if (j != 0)
					{
						query1 << " not null default 0";
					}
				}
				else if (files_vec_[i].itstr_vec[j].type == "int32")
				{
					query1 << "int not null default 0";
				}
				else if (files_vec_[i].itstr_vec[j].type == "uint32")
				{
					query1 << "int not null default 0";
				}
				else if (files_vec_[i].itstr_vec[j].type == "bytes")
				{
					query1 << "mediumblob";
				}
				else if (files_vec_[i].itstr_vec[j].type == "string")
				{
					query1 << "text";
				}
				else if (files_vec_[i].itstr_vec[j].type == "dt")
				{
					query1 << "timestamp not null default now()";
				}
				else if (files_vec_[i].itstr_vec[j].type == "float")
				{
					query1 << "double not null default 0";
				}
				else if (files_vec_[i].itstr_vec[j].type == "double")
				{
					query1 << "double not null default 0";
				}

				if (j == 0)
				{
					if (files_vec_[i].use_auto_increment)
					{
						query1 << " auto_increment";
					}
					query1 << " PRIMARY KEY";
				}
				if (j != files_vec_[i].itstr_vec.size() - 1)
				{
					query1 << ", ";
				}
			}
			query1 << ")";

			query1.execute();

			//////////////////////////////////////////////////////////////////////////

			for (int j = 0; j < files_vec_[i].indexes.size(); ++j)
			{
				mysqlpp::Query query2 = conn->query();
				query2 << "CREATE INDEX ";
				query2 << files_vec_[i].indexes[j].index_name;
				query2 << "_index ON ";
				query2 << files_vec_[i].cname;
				query2 << " (";
				query2 << files_vec_[i].indexes[j].index_name;
				if (files_vec_[i].indexes[j].index_len)
				{
					query2 << "(";
					query2 << boost::lexical_cast<std::string>(files_vec_[i].indexes[j].index_len);
					query2 << ")";
				}
				query2 << ")";

				query2.execute();
			}

			for (int j = 0; j < files_vec_[i].uniques.size(); ++j)
			{
				mysqlpp::Query query2 = conn->query();
				query2 << "ALTER TABLE ";
				query2 << files_vec_[i].cname;
				query2 << " ADD UNIQUE ";
				query2 << files_vec_[i].uniques[j].index_name;
				if (files_vec_[i].uniques[j].index_len)
				{
					query2 << "(";
					query2 << boost::lexical_cast<std::string>(files_vec_[i].uniques[j].index_len);
					query2 << ")";
				}

				query2.execute();
			}
		}
	}

	conn->disconnect();

	return 0;
}

int write_sql3(const std::string &db, const std::string &ip, const std::string &user, const std::string &pwd, const std::string &mode)
{
	if (mode == "0")
	{
		if (write_sql(db, ip, user, pwd) == -1)
		{
			return -1;
		}
	}
	else if (mode == "1")
	{
		if (write_sql2(db, ip, user, pwd) == -1)
		{
			return -1;
		}
	}

	return 0;
}

int main(int argc, char *argv[])
{
	if (argc < 7)
	{
		return 0;
	}
	if (read_proto(argv[1]) == -1)
	{
		return -1;
	}

	if (write_sql3(argv[2], argv[3], argv[4], argv[5], argv[6]) == -1)
	{
		return -1;
	}
	return 0;
}
