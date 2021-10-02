function ChangeDateFormat(d: string): String {
  let d2: Date = new Date(d);
  const dd = String(d2.getDate()).padStart(2, '0');
  const mm = String(d2.getMonth() + 1).padStart(2, '0'); //January is 0!
  const yyyy = d2.getFullYear();
  return dd + '-' + mm + '-' + yyyy;
}

export { ChangeDateFormat };
