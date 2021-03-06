/* tslint:disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v10.0.6306.29915 (NJsonSchema v8.30.6304.31883) (http://NSwag.org)
// </auto-generated>
//----------------------


class CompaniesClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: (key: string, value: any) => any = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.baseUrl = baseUrl ? baseUrl : "";
        this.http = http ? http : <any>window;
    }

    get(): Promise<Company[]> {
        let url_ = this.baseUrl + "/api/Companies";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            headers: {
                "Content-Type": "application/json; charset=UTF-8", 
                "Accept": "application/json; charset=UTF-8"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGet(_response);
        });
    }

    protected processGet(_response: Response): Promise<Company[]> {
        const _status = _response.status;
        if (_status === 200) {
            return _response.text().then((_responseText) => {
            let result200: Company[] = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (resultData200 && resultData200.constructor === Array) {
                result200 = [];
                for (let item of resultData200)
                    result200.push(Company.fromJS(item));
            }
            return result200;
            });
        } else if (_status !== 200 && _status !== 204) {
            return _response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", _status, _responseText);
            });
        }
        return Promise.resolve(null);
    }
}

class OrdersClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: (key: string, value: any) => any = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.baseUrl = baseUrl ? baseUrl : "";
        this.http = http ? http : <any>window;
    }

    get(companyId: number, pageIndex: number, pageSize: number): Promise<Order[]> {
        let url_ = this.baseUrl + "/api/Orders/{companyId}?";
        if (companyId === undefined || companyId === null)
            throw new Error("The parameter 'companyId' must be defined.");
        url_ = url_.replace("{companyId}", encodeURIComponent("" + companyId)); 
        if (pageIndex === null)
            throw new Error("The parameter 'pageIndex' cannot be null.");
        else if (pageIndex !== undefined)
            url_ += "pageIndex=" + encodeURIComponent("" + pageIndex) + "&"; 
        if (pageSize === null)
            throw new Error("The parameter 'pageSize' cannot be null.");
        else if (pageSize !== undefined)
            url_ += "pageSize=" + encodeURIComponent("" + pageSize) + "&"; 
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            headers: {
                "Content-Type": "application/json; charset=UTF-8", 
                "Accept": "application/json; charset=UTF-8"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGet(_response);
        });
    }

    protected processGet(_response: Response): Promise<Order[]> {
        const _status = _response.status;
        if (_status === 200) {
            return _response.text().then((_responseText) => {
            let result200: Order[] = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (resultData200 && resultData200.constructor === Array) {
                result200 = [];
                for (let item of resultData200)
                    result200.push(Order.fromJS(item));
            }
            return result200;
            });
        } else if (_status !== 200 && _status !== 204) {
            return _response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", _status, _responseText);
            });
        }
        return Promise.resolve(null);
    }

    getItem(orderId: number): Promise<Order> {
        let url_ = this.baseUrl + "/api/Orders/{orderId}";
        if (orderId === undefined || orderId === null)
            throw new Error("The parameter 'orderId' must be defined.");
        url_ = url_.replace("{orderId}", encodeURIComponent("" + orderId)); 
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <RequestInit>{
            method: "GET",
            headers: {
                "Content-Type": "application/json; charset=UTF-8", 
                "Accept": "application/json; charset=UTF-8"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetItem(_response);
        });
    }

    protected processGetItem(_response: Response): Promise<Order> {
        const _status = _response.status;
        if (_status === 200) {
            return _response.text().then((_responseText) => {
            let result200: Order = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 ? Order.fromJS(resultData200) : <any>null;
            return result200;
            });
        } else if (_status !== 200 && _status !== 204) {
            return _response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", _status, _responseText);
            });
        }
        return Promise.resolve(null);
    }

    put(orderId: number, order: Order): Promise<Blob> {
        let url_ = this.baseUrl + "/api/Orders/{orderId}";
        if (orderId === undefined || orderId === null)
            throw new Error("The parameter 'orderId' must be defined.");
        url_ = url_.replace("{orderId}", encodeURIComponent("" + orderId)); 
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(order ? order.toJSON() : null);

        let options_ = <RequestInit>{
            body: content_,
            method: "PUT",
            headers: {
                "Content-Type": "application/json; charset=UTF-8", 
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processPut(_response);
        });
    }

    protected processPut(_response: Response): Promise<Blob> {
        const _status = _response.status;
        if (_status === 200) {
            return _response.blob();
        } else if (_status !== 200 && _status !== 204) {
            return _response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", _status, _responseText);
            });
        }
        return Promise.resolve(null);
    }

    post(order: Order): Promise<Blob> {
        let url_ = this.baseUrl + "/api/Orders";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(order ? order.toJSON() : null);

        let options_ = <RequestInit>{
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json; charset=UTF-8", 
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processPost(_response);
        });
    }

    protected processPost(_response: Response): Promise<Blob> {
        const _status = _response.status;
        if (_status === 200) {
            return _response.blob();
        } else if (_status !== 200 && _status !== 204) {
            return _response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", _status, _responseText);
            });
        }
        return Promise.resolve(null);
    }

    delete(orderId: number): Promise<void> {
        let url_ = this.baseUrl + "/api/Orders/delete-{orderId}";
        if (orderId === undefined || orderId === null)
            throw new Error("The parameter 'orderId' must be defined.");
        url_ = url_.replace("{orderId}", encodeURIComponent("" + orderId)); 
        url_ = url_.replace(/[?&]$/, "");

        const content_ = "";

        let options_ = <RequestInit>{
            body: content_,
            method: "DELETE",
            headers: {
                "Content-Type": "application/json; charset=UTF-8", 
                "Accept": "application/json; charset=UTF-8"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processDelete(_response);
        });
    }

    protected processDelete(_response: Response): Promise<void> {
        const _status = _response.status;
        if (_status === 204) {
            return _response.text().then((_responseText) => {
            return null;
            });
        } else if (_status !== 200 && _status !== 204) {
            return _response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", _status, _responseText);
            });
        }
        return Promise.resolve(null);
    }
}

class Company implements ICompany {
    id: number;
    name?: string;
    owner?: string;

    constructor(data?: ICompany) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["Id"];
            this.name = data["Name"];
            this.owner = data["Owner"];
        }
    }

    static fromJS(data: any): Company {
        let result = new Company();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = data ? data : {};
        data["Id"] = this.id;
        data["Name"] = this.name;
        data["Owner"] = this.owner;
        return data; 
    }
}

interface ICompany {
    id: number;
    name?: string;
    owner?: string;
}

class Order implements IOrder {
    id: number;
    number?: string;
    date: Date;
    companyId: number;
    orderItems?: OrderItem[];

    constructor(data?: IOrder) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["Id"];
            this.number = data["Number"];
            this.date = data["Date"] ? new Date(data["Date"].toString()) : <any>undefined;
            this.companyId = data["CompanyId"];
            if (data["OrderItems"] && data["OrderItems"].constructor === Array) {
                this.orderItems = [];
                for (let item of data["OrderItems"])
                    this.orderItems.push(OrderItem.fromJS(item));
            }
        }
    }

    static fromJS(data: any): Order {
        let result = new Order();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = data ? data : {};
        data["Id"] = this.id;
        data["Number"] = this.number;
        data["Date"] = this.date ? this.date.toISOString() : <any>undefined;
        data["CompanyId"] = this.companyId;
        if (this.orderItems && this.orderItems.constructor === Array) {
            data["OrderItems"] = [];
            for (let item of this.orderItems)
                data["OrderItems"].push(item.toJSON());
        }
        return data; 
    }
}

interface IOrder {
    id: number;
    number?: string;
    date: Date;
    companyId: number;
    orderItems?: OrderItem[];
}

class OrderItem implements IOrderItem {
    id: number;
    text?: string;
    amount: number;
    discount?: number;
    isOnStock: boolean;

    constructor(data?: IOrderItem) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(data?: any) {
        if (data) {
            this.id = data["Id"];
            this.text = data["Text"];
            this.amount = data["Amount"];
            this.discount = data["Discount"];
            this.isOnStock = data["IsOnStock"];
        }
    }

    static fromJS(data: any): OrderItem {
        let result = new OrderItem();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = data ? data : {};
        data["Id"] = this.id;
        data["Text"] = this.text;
        data["Amount"] = this.amount;
        data["Discount"] = this.discount;
        data["IsOnStock"] = this.isOnStock;
        return data; 
    }
}

interface IOrderItem {
    id: number;
    text?: string;
    amount: number;
    discount?: number;
    isOnStock: boolean;
}

class SwaggerException extends Error {
    message: string;
    status: number; 
    response: string; 
    result: any; 

    constructor(message: string, status: number, response: string, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.result = result;
    }
}

function throwException(message: string, status: number, response: string, result?: any): any {
    if(result !== null && result !== undefined)
        throw result;
    else
        throw new SwaggerException(message, status, response, null);
}