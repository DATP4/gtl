#[cfg(test)]
mod tests {
#[test]
fn declaration_test1(){
let x = 5
;
assert_eq!(x, 5)
}
#[test]
fn declaration_test2(){
let x = 5.0
;
assert_eq!(x, 5.0);
}
#[test]
fn declaration_test3(){
let x = true
;
assert_eq!(x, true);
}
}