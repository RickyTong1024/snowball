package com.yymoon.snowball;

import android.text.TextUtils;

import java.util.Map;

public class PayResult {
    private String resultStatus;
    private String result;
    private String memo;

    public PayResult(String rawResult)
    {
        if (TextUtils.isEmpty(rawResult)) {
            return;
        }
        String[] resultParams = rawResult.split(";");
        for (String resultParam : resultParams)
        {
            if (resultParam.startsWith("resultStatus")) {
                this.resultStatus = gatValue(resultParam, "resultStatus");
            }
            if (resultParam.startsWith("result")) {
                this.result = gatValue(resultParam, "result");
            }
            if (resultParam.startsWith("memo")) {
                this.memo = gatValue(resultParam, "memo");
            }
        }
    }

    public String toString() {
        return "resultStatus={" + this.resultStatus + "};memo={" + this.memo + "};result={" + this.result + "}";
    }

    private String gatValue(String content, String key) {
        String prefix = key + "={";
        return content.substring(content.indexOf(prefix) + prefix.length(),
                content.lastIndexOf("}"));
    }

    public String getResultStatus() {
        return this.resultStatus;
    }

    public String getMemo() {
        return this.memo;
    }

    public String getResult() {
        return this.result;
    }
}
