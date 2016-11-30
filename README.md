# ZoDream Spider

## 主要结构

> 主页面 显示所有网址及执行情况  包括停止开始

    操作:
    1.任务保存载入
    2.停止继续暂停开始

> 添加页面 初始网址 规则列表 请求头设置列表

    参数：
    1.网址 【】
    2.线程数 【】
    3.等待时间 【】
    4.是否使用浏览器 【】
    5.保存路径 【】
    
    操作：
    1.规则管理
    2.规则保存载入

> 规则页面 网址匹配 及对应的规则

    具体规则:
    1.正则截取  【】
    2.普通截取  【】 【】
    3.普通替换  【】 【】
    4.正则替换  【】 【】
    5.正则匹配  【】 【提取标签】
    6.简繁转换  【Y/N】 Y或为空时表示 繁体转简体 其他字符表示简体转繁体
    7.XPath选择 【】 有多个结果时以换行符拼接
    8.Csv保存   【正则】【路径】 正则必须带标签，第一行为标签
    9.Excel保存 【正则】【路径】同上
    10.保存     【】 【模板路径】 支持保存为html txt(特指5匹配得到的列表) 如果为文件直接执行本操作 
    11.导入     【】 【post参数】 支持接口导入 {url}表示网址 {content}表示内容 {}表示正则标签
    12.追加     【】【模板路径】 
    
    操作：
    1.具体规则管理

    说明：
    1.路径 支持正则匹配标签 {}，有默认地址，所有替换的地址是相对默认地址路径，使用相对路径是基于保存路径

    
> 浏览器页面
    
    自动获取请求头
    
## 流程

    加载初始网址
    装配下载请求头
    获取源码
    提取url 并替换url为相对路径
    循环匹配url规则
        循环执行对应规则
        保存

## 已实现功能

    1.多线程下载
    2.支持浏览器下载
    3.支持多格式保存
    4.支持多种规则提取

## 已知问题

    1.使用浏览器下载时，不支持文件保存
    2.多线程追加问题

## 待开发功能

    1.自定义网址提取

## 更新时间

    2016/11/27 15:33